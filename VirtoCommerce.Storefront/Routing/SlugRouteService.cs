using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Routing
{
    public class SlugRouteService : ISlugRouteService
    {
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly ICommerce _coreApi;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public SlugRouteService(IStorefrontMemoryCache memoryCache, ICommerce coreApi, IApiChangesWatcher apiChangesWatcher)
        {
            _memoryCache = memoryCache;
            _coreApi = coreApi;
            _apiChangesWatcher = apiChangesWatcher;
        }

        public virtual async Task<SlugRouteResponse> HandleSlugRequestAsync(string slugPath, WorkContext workContext)
        {
            var entity = await FindEntityBySlugPath(slugPath, workContext) ?? new SlugRoutingData { ObjectType = "Asset", SeoPath = slugPath };
            var response = entity.SeoPath.EqualsInvariant(slugPath) ? View(entity) : Redirect(entity);
            return response;
        }

        protected virtual SlugRouteResponse View(SlugRoutingData routingData)
        {
            var response = new SlugRouteResponse();

            switch (routingData.ObjectType)
            {
                case "Category":
                    response.RouteData["action"] = "ThemeView";
                    response.RouteData["controller"] = "ThemeView";
                    response.RouteData["routing"] = routingData;
                    response.RouteData["viewName"] = "collection";
                    break;
                case "CatalogProduct":
                    response.RouteData["action"] = "ThemeView";
                    response.RouteData["controller"] = "ThemeView";
                    response.RouteData["routing"] = routingData;
                    response.RouteData["viewName"] = "product";
                    break;
                case "Page":
                    response.RouteData["action"] = "GetContentPage";
                    response.RouteData["controller"] = "StaticContent";
                    response.RouteData["page"] = routingData.ObjectInstance;
                    break;
                case "Asset":
                    response.RouteData["action"] = "GetThemeAssets";
                    response.RouteData["controller"] = "Asset";
                    response.RouteData["path"] = routingData.SeoPath;
                    break;
            }

            return response;
        }

        protected virtual SlugRouteResponse Redirect(SlugRoutingData entity)
        {
            return new SlugRouteResponse
            {
                Redirect = true,
                RedirectLocation = entity.SeoPath,
            };
        }

        protected virtual async Task<SlugRoutingData> FindEntityBySlugPath(string path, WorkContext workContext)
        {
            path = path.Trim('/');

            var slugs = path.Split('/');
            var lastSlug = slugs.LastOrDefault();

            // Get all SEO records for requested slug and also all other SEO records with different slug and languages but related to the same object
            var allSeoRecords = await GetAllSeoRecordsAsync(lastSlug);
            var bestSeoRecords = GetBestMatchingSeoRecords(allSeoRecords, workContext.CurrentStore, workContext.CurrentLanguage, lastSlug);

            var routingComparer = AnonymousComparer.Create((SlugRoutingData x) => string.Join(":", x.ObjectType, x.ObjectId, x.SeoPath));
            // Find distinct objects
            var entities = bestSeoRecords
                .Select(s => new SlugRoutingData { ObjectType = s.ObjectType, ObjectId = s.ObjectId, SeoPath = s.SemanticUrl })
                .Distinct(routingComparer)
                .ToList();

            // Don't load objects for non-SEO links
            if (workContext.CurrentStore.SeoLinksType != SeoLinksType.None)
            {
                foreach (var group in entities.GroupBy(e => e.ObjectType))
                {
                    await LoadObjectsAndBuildFullSeoPaths(group.Key, group.ToList(), workContext.CurrentStore, workContext.CurrentLanguage);
                }

                entities = entities.Where(e => !string.IsNullOrEmpty(e.SeoPath)).ToList();
            }

            // If found multiple entities, keep those which have the requested SEO path
            if (entities.Count > 1)
            {
                entities = entities.Where(e => e.SeoPath.EqualsInvariant(path)).ToList();
            }

            // If still found multiple entities, give up
            var result = entities.Count == 1 ? entities.FirstOrDefault() : null;

            if (result == null)
            {
                // Try to find a static page
                var page = FindPageBySeoPath(path, workContext);
                if (page != null)
                {
                    result = new SlugRoutingData
                    {
                        ObjectType = "Page",
                        SeoPath = page.Url,
                        ObjectInstance = page,
                    };
                }
            }

            return result;
        }

        protected virtual ContentItem FindPageBySeoPath(string seoPath, WorkContext workContext)
        {
            ContentItem result = null;

            if (workContext.Pages != null)
            {
                var pages = workContext.Pages
                    .Where(p => string.Equals(p.Url, seoPath, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Find page with current language
                result = pages.FirstOrDefault(x => x.Language == workContext.CurrentLanguage);

                if (result == null)
                {
                    // Find page with invariant language
                    result = pages.FirstOrDefault(x => x.Language.IsInvariant);
                }

                if (result == null)
                {
                    // Check alternate page URLs
                    result = workContext.Pages.FirstOrDefault(x => x.AliasesUrls.Contains(seoPath, StringComparer.OrdinalIgnoreCase));
                }
            }

            return result;
        }

        protected virtual async Task<IList<coreDto.SeoInfo>> GetAllSeoRecordsAsync(string slug)
        {
            var result = new List<coreDto.SeoInfo>();

            if (!string.IsNullOrEmpty(slug))
            {
                var cacheKey = CacheKey.With(GetType(), "GetAllSeoRecords", slug);
                var apiResult = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
                {
                    cacheEntry.AddExpirationToken(RoutingCacheRegion.CreateChangeToken());
                    cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());
                    return await _coreApi.GetSeoInfoBySlugAsync(slug);
                });
                result.AddRange(apiResult);
            }
            return result;
        }

        protected virtual IList<coreDto.SeoInfo> GetBestMatchingSeoRecords(IEnumerable<coreDto.SeoInfo> allSeoRecords, Store store, Language language, string slug)
        {
            return allSeoRecords.GetBestMatchingSeoInfos(store, language, slug);
        }

        protected virtual async Task LoadObjectsAndBuildFullSeoPaths(string objectType, IList<SlugRoutingData> objects, Store store, Language language)
        {
            var objectIds = objects.Select(o => o.ObjectId).ToArray();
            var seoPaths = await GetFullSeoPathsAsync(objectType, objectIds, store, language);

            if (seoPaths != null)
            {
                foreach (var seo in objects)
                {
                    seo.SeoPath = seoPaths[seo.ObjectId] ?? seo.SeoPath;
                }
            }
        }

        protected virtual async Task<IDictionary<string, string>> GetFullSeoPathsAsync(string objectType, string[] objectIds, Store store, Language language)
        {
            var cacheKey = CacheKey.With(GetType(), "GetFullSeoPaths", store.Id, objectType, string.Join("-", objectIds.OrderBy(x => x)));
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(RoutingCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());
                switch (objectType)
                {
                    //TODO:
                    //case "Category":
                    //    return await GetCategorySeoPathsAsync(objectIds, store, language);
                    //case "CatalogProduct":
                    //    return await GetProductSeoPathsAsync(objectIds, store, language);
                }
                //TODO:return proper dict
                return Task.FromResult(new Dictionary<string, string>().WithDefaultValue(null));
            });
        }

        //TODO:
        //protected virtual async Task<IDictionary<string, string>> GetCategorySeoPathsAsync(string[] objectIds, Store store, Language language)
        //{
        //    var result = (await _catalogService.GetCategoriesAsync(objectIds, CategoryResponseGroup.WithOutlines | CategoryResponseGroup.WithSeo));
        //    return result.ToDictionary(x => x.Id, x => x.SeoPath).WithDefaultValue(null);
        //}

        //protected virtual async Task<IDictionary<string, string>> GetProductSeoPathsAsync(string[] objectIds, Store store, Language language)
        //{
        //    var result = await _catalogService.GetProductsAsync(objectIds, ItemResponseGroup.Outlines | ItemResponseGroup.Seo);
        //    return result.ToDictionary(x => x.Id, x => x.SeoPath).WithDefaultValue(null);
        //}
    }
}
