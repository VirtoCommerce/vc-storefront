using CacheManager.Core;
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

        public async Task<MenuLinkList[]> LoadAllStoreLinkListsAsync(string storeId)
        {
            return await _cacheManager.GetAsync($"LoadAllStoreLinkListsAsync-{storeId}", StorefrontConstants.MenuLinksCacheRegion, async () =>
            {
                var retVal = new List<MenuLinkList>();
                var linkLists = await _cmsApi.GetListsAsync(storeId);
                if (linkLists != null)
                {
                    retVal.AddRange(linkLists.Select(x => x.ToMenuLinkList()));

                    var allMenuLinks = retVal.SelectMany(x => x.MenuLinks).ToList();
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

                }
                return retVal.ToArray();
            });
        }
    }
}
