using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.ContentModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.LinkList.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Infrastructure;

namespace VirtoCommerce.Storefront.Domain
{
    public class MenuLinkListServiceImpl : IMenuLinkListService
    {
        private readonly IMenu _cmsApi;
        private readonly ICatalogService _catalogService;
        private readonly IMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;
        public MenuLinkListServiceImpl(IMenu cmsApi, ICatalogService catalogService, IMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher)
        {
            _cmsApi = cmsApi;
            _catalogService = catalogService;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
        }

        public async Task<IList<MenuLinkList>> LoadAllStoreLinkListsAsync(Store store, Language language)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            var cacheKey = CacheKey.With(GetType(), "LoadAllStoreLinkListsAsync", store.Id, language.CultureName);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(StaticContentCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                var result = new List<MenuLinkList>();
                var listsDto = await _cmsApi.GetListsAsync(store.Id);
                if(listsDto != null)
                {
                    result.AddRange(listsDto.Select(x => x.ToMenuLinkList()));
                }

                result = result.GroupBy(x => x.Name).Select(x => x.FindWithLanguage(language)).Where(x => x != null).ToList().ToList();

                var allMenuLinks = result.SelectMany(x => x.MenuLinks).ToList();
                var productLinks = allMenuLinks.OfType<ProductMenuLink>().ToList();
                var categoryLinks = allMenuLinks.OfType<CategoryMenuLink>().ToList();

                Task<Product[]> productsLoadingTask = null;
                Task<Category[]> categoriesLoadingTask = null;

                //Parallel loading associated objects
                var productIds = productLinks.Select(x => x.AssociatedObjectId).ToArray();
                if (productIds.Any())
                {
                    productsLoadingTask = _catalogService.GetProductsAsync(productIds, ItemResponseGroup.ItemSmall);
                }
                var categoriesIds = categoryLinks.Select(x => x.AssociatedObjectId).ToArray();
                if (categoriesIds.Any())
                {
                    categoriesLoadingTask = _catalogService.GetCategoriesAsync(categoriesIds, CategoryResponseGroup.Info | CategoryResponseGroup.WithImages | CategoryResponseGroup.WithSeo | CategoryResponseGroup.WithOutlines);
                }
                //Populate link by associated product
                if (productsLoadingTask != null)
                {
                    var products = await productsLoadingTask;
                    foreach (var productLink in productLinks)
                    {
                        productLink.Product = products.FirstOrDefault(x => x.Id == productLink.AssociatedObjectId);
                    }
                }
                //Populate link by associated category
                if (categoriesLoadingTask != null)
                {
                    var categories = await categoriesLoadingTask;
                    foreach (var categoryLink in categoryLinks)
                    {
                        categoryLink.Category = categories.FirstOrDefault(x => x.Id == categoryLink.AssociatedObjectId);
                    }
                }

                return result.ToList();
            });
        }
    }
}
