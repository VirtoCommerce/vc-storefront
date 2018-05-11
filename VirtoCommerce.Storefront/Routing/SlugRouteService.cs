using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Routing
{
    public class SlugRouteService : ISlugRouteService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly  ICommerce _coreApi;
        private readonly ICatalogService _catalogService;

        public SlugRouteService(IMemoryCache memoryCache, ICommerce coreApi, ICatalogService catalogService, ICatalogModuleProducts catalogProductsApi)
        {
            _memoryCache = memoryCache;
            _coreApi = coreApi;
            _catalogService = catalogService;
        }

        public virtual async Task<SlugRouteResponse> HandleSlugRequestAsync(string slugPath, WorkContext workContext)
        {
            var entity = await FindEntityBySlugPath(slugPath, workContext) ?? new SeoEntity { ObjectType = "Asset", SeoPath = slugPath };
            var response = entity.SeoPath.EqualsInvariant(slugPath) ? View(entity) : Redirect(entity);
            return response;
        }


        protected virtual SlugRouteResponse View(SeoEntity entity)
        {
            var response = new SlugRouteResponse();

            switch (entity.ObjectType)
            {
                case "Category":
                    response.RouteData["action"] = "CategoryBrowsing";
                    response.RouteData["controller"] = "CatalogSearch";
                    response.RouteData["categoryId"] = entity.ObjectId;
                    break;
                case "CatalogProduct":
                    response.RouteData["action"] = "ProductDetails";
                    response.RouteData["controller"] = "Product";
                    response.RouteData["productId"] = entity.ObjectId;
                    break;
                case "Vendor":
                    response.RouteData["action"] = "VendorDetails";
                    response.RouteData["controller"] = "Vendor";
                    response.RouteData["vendorId"] = entity.ObjectId;
                    break;
                case "Page":
                    response.RouteData["action"] = "GetContentPage";
                    response.RouteData["controller"] = "StaticContent";
                    response.RouteData["page"] = entity.ObjectInstance;
                    break;
                case "Asset":
                    response.RouteData["action"] = "GetThemeAssets";
                    response.RouteData["controller"] = "Asset";
                    response.RouteData["path"] = entity.SeoPath;
                    break;
            }

            return response;
        }

        protected virtual SlugRouteResponse Redirect(SeoEntity entity)
        {
            return new SlugRouteResponse
            {
                Redirect = true,
                RedirectLocation = entity.SeoPath,
            };
        }

        protected virtual async Task<SeoEntity> FindEntityBySlugPath(string path, WorkContext workContext)
        {
            path = path.Trim('/');

            var slugs = path.Split('/');
            var lastSlug = slugs.LastOrDefault();

            // Get all SEO records for requested slug and also all other SEO records with different slug and languages but related to the same object
            var allSeoRecords = await GetAllSeoRecordsAsync(lastSlug);
            var bestSeoRecords = GetBestMatchingSeoRecords(allSeoRecords, workContext.CurrentStore, workContext.CurrentLanguage, lastSlug);

            var seoEntityComparer = AnonymousComparer.Create((SeoEntity x) => string.Join(":", x.ObjectType, x.ObjectId, x.SeoPath));
            // Find distinct objects
            var entities = bestSeoRecords
                .Select(s => new SeoEntity { ObjectType = s.ObjectType, ObjectId = s.ObjectId, SeoPath = s.SemanticUrl })
                .Distinct(seoEntityComparer)
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
                    result = new SeoEntity
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

        protected virtual async Task LoadObjectsAndBuildFullSeoPaths(string objectType, IList<SeoEntity> objects, Store store, Language language)
        {
            var objectIds = objects.Select(o => o.ObjectId).ToArray();
            var seoPaths = await GetFullSeoPathsAsync(objectType, objectIds, store, language);

            if (seoPaths != null)
            {
                foreach (var seo in objects)
                {
                    if (seoPaths.ContainsKey(seo.ObjectId))
                    {
                        seo.SeoPath = seoPaths[seo.ObjectId];
                    }
                }
            }
        }

        protected virtual async  Task<IDictionary<string, string>> GetFullSeoPathsAsync(string objectType, string[] objectIds, Store store, Language language)
        {
            var cacheKey = CacheKey.With(GetType(), "GetFullSeoPaths", store.Id, objectType, objectIds.GetOrderIndependentHashCode().ToString());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                switch (objectType)
                {
                    case "Category":
                        return await GetCategorySeoPathsAsync(objectIds, store, language);
                    case "CatalogProduct":
                        return await GetProductSeoPathsAsync(objectIds, store, language);
                }
                return new Dictionary<string, string>();
            });           
        }

        protected virtual async Task<IDictionary<string, string>> GetCategorySeoPathsAsync(string[] objectIds, Store store, Language language)
        {
            var result = (await _catalogService.GetCategoriesAsync(objectIds, CategoryResponseGroup.WithOutlines | CategoryResponseGroup.WithSeo));
            return result.ToDictionary(x => x.Id, x => x.SeoPath);
        }

        protected virtual async Task<IDictionary<string, string>> GetProductSeoPathsAsync(string[] objectIds, Store store, Language language)
        {
            var result = await _catalogService.GetProductsAsync(objectIds, ItemResponseGroup.Outlines | ItemResponseGroup.Seo);
            return result.ToDictionary(x => x.Id, x => x.SeoPath);
        }
    }
}
