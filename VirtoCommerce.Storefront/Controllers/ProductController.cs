using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
    public class ProductController : StorefrontControllerBase
    {
        private readonly ICatalogService _catalogSearchService;

        public ProductController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ICatalogService catalogSearchService)
            : base(workContextAccessor, urlBuilder)
        {
            _catalogSearchService = catalogSearchService;
        }

        /// <summary>
        /// This action used by storefront to get product details by product id
        /// </summary>
        /// <param name="productId"></param>
        [HttpGet("product/{productId}")]
        public async Task<ActionResult> ProductDetails(string productId)
        {
            var product = (await _catalogSearchService.GetProductsAsync(new[] { productId }, WorkContext.CurrentProductResponseGroup)).FirstOrDefault();
            WorkContext.CurrentProduct = product;

            if (product != null)
            {
                WorkContext.CurrentPageSeo = product.SeoInfo.JsonClone();
                WorkContext.CurrentPageSeo.Slug = product.Url;

                // make sure title is set
                if (string.IsNullOrEmpty(WorkContext.CurrentPageSeo.Title))
                {
                    WorkContext.CurrentPageSeo.Title = product.Name;
                }

                if (product.CategoryId != null)
                {
                    var category = (await _catalogSearchService.GetCategoriesAsync(new[] { product.CategoryId }, CategoryResponseGroup.Full)).FirstOrDefault();
                    WorkContext.CurrentCategory = category;

                    if (category != null)
                    {
                        category.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos, @params) =>
                        {
                            var criteria = WorkContext.CurrentProductSearchCriteria.Clone() as ProductSearchCriteria;
                            criteria.Outline = product.GetCategoryOutline();
                            criteria.PageNumber = pageNumber;
                            criteria.PageSize = pageSize;
                            if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos.IsNullOrEmpty())
                            {
                                criteria.SortBy = SortInfo.ToString(sortInfos);
                            }
                            if (@params != null)
                            {
                                criteria.CopyFrom(@params);
                            }
                            return _catalogSearchService.SearchProducts(criteria).Products;
                        }, 1, ProductSearchCriteria.DefaultPageSize);
                    }
                }
            }
            return View("product", WorkContext);
        }

        [HttpGet("compare")]
        public ActionResult Compare()
        {
            return View("product-compare");
        }
    }
}
