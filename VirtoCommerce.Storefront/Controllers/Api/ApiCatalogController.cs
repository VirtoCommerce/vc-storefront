using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiCatalogController : StorefrontControllerBase
    {
        private readonly ICatalogService _catalogService;
        public ApiCatalogController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ICatalogService catalogSearchService)
            : base(workContextAccessor, urlBuilder)
        {
            _catalogService = catalogSearchService;
        }

        // storefrontapi/catalog/search
        [HttpPost("catalog/search")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<SearchProductsResult>> SearchProducts([FromBody] ProductSearchCriteria searchCriteria)
        {
            var retVal = await _catalogService.SearchProductsAsync(searchCriteria);
            foreach (var product in retVal.Products)
            {
                product.Url = base.UrlBuilder.ToAppAbsolute(product.Url);
            }
            return new SearchProductsResult
            {
                Products = retVal.Products,
                Aggregations = retVal.Aggregations,
                MetaData = retVal.Products.GetMetaData()
            };
        }

        // storefrontapi/products?productIds=...&respGroup=...
        [HttpGet("products")]
        public async Task<ActionResult<Product[]>> GetProductsByIds(string[] productIds, ItemResponseGroup respGroup = ItemResponseGroup.ItemLarge)
        {
            return await _catalogService.GetProductsAsync(productIds, respGroup);
        }

        // storefrontapi/categories/search
        [HttpPost("categories/search")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<SearchCategoriesResult>> SearchCategories([FromBody] CategorySearchCriteria searchCriteria)
        {
            var retVal = await _catalogService.SearchCategoriesAsync(searchCriteria);
            foreach (var category in retVal)
            {
                category.Url = base.UrlBuilder.ToAppAbsolute(category.Url);
            }
            return new SearchCategoriesResult
            {
                Categories = retVal,
                MetaData = retVal.GetMetaData()
            };
        }

        // GET: storefrontapi/categories
        [HttpGet("categories")]
        public async Task<ActionResult<Category[]>> GetCategoriesByIds(string[] categoryIds, CategoryResponseGroup respGroup = CategoryResponseGroup.Full)
        {
            return await _catalogService.GetCategoriesAsync(categoryIds, respGroup);
        }
    }
}
