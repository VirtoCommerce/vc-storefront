using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiCatalogController : StorefrontControllerBase
    {
        private readonly ICatalogService _catalogService;
        public ApiCatalogController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ICatalogService catalogSearchService)
            : base(workContextAccessor, urlBuilder)
        {
            _catalogService = catalogSearchService;
        }

        // storefrontapi/catalog/search
        [HttpPost]
        public async Task<ActionResult> SearchProducts([FromBody] ProductSearchCriteria searchCriteria)
        {
            var retVal = await _catalogService.SearchProductsAsync(searchCriteria);
            foreach (var product in retVal.Products)
            {
                product.Url = base.UrlBuilder.ToAppAbsolute(product.Url);
            }
            return Json(new
            {
                Products = retVal.Products,
                Aggregations = retVal.Aggregations,
                MetaData = retVal.Products.GetMetaData()
            });
        }

        // storefrontapi/products?productIds=...&respGroup=...
        [HttpGet]
        public async Task<ActionResult> GetProductsByIds(string[] productIds, ItemResponseGroup respGroup = ItemResponseGroup.ItemLarge)
        {
            var retVal = await _catalogService.GetProductsAsync(productIds, respGroup);
            return Json(retVal);
        }

        // storefrontapi/categories/search
        [HttpPost]
        public async Task<ActionResult> SearchCategories([FromBody] CategorySearchCriteria searchCriteria)
        {
            var retVal = await _catalogService.SearchCategoriesAsync(searchCriteria);
            foreach (var category in retVal)
            {
                category.Url = base.UrlBuilder.ToAppAbsolute(category.Url);
            }
            return Json(new
            {
                Categories = retVal,
                MetaData = retVal.GetMetaData()
            });
        }

        // GET: storefrontapi/categories
        [HttpGet]
        public async Task<ActionResult> GetCategoriesByIds(string[] categoryIds, CategoryResponseGroup respGroup = CategoryResponseGroup.Full)
        {
            var retVal = await _catalogService.GetCategoriesAsync(categoryIds, respGroup);
            return Json(retVal);
        }

    }
}
