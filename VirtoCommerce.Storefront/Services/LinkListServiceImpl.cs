using CacheManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.ContentModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.LinkList.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Services
{
    public class MenuLinkListServiceImpl : IMenuLinkListService
    {
        private readonly IMenu _cmsApi;
        private readonly ICatalogService _catalogService;
        private readonly ICacheManager<object> _cacheManager;

        public MenuLinkListServiceImpl(IMenu cmsApi, ICatalogService catalogService, ICacheManager<object> cacheManager)
        {
            _cmsApi = cmsApi;
            _catalogService = catalogService;
            _cacheManager = cacheManager;
        }

        public async Task<IList<MenuLinkList>> LoadAllStoreLinkListsAsync(Store store, Language language)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            return await _cacheManager.GetAsync($"LoadAllStoreLinkListsAsync-{store.Id}", StorefrontConstants.MenuLinksCacheRegion, async () =>
            {
                var result = (await _cmsApi.GetListsAsync(store.Id)).Select(x => x.ToMenuLinkList());
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
