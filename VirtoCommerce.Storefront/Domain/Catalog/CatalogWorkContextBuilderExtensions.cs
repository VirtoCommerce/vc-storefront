using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public static class CatalogWorkContextBuilderExtensions
    {
        public static Task WithCatalogsAsync(this IWorkContextBuilder builder)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var catalogService = serviceProvider.GetRequiredService<ICatalogService>();

            //Initialize catalog search criteria
            builder.WorkContext.CurrentProductSearchCriteria = new ProductSearchCriteria(builder.WorkContext.CurrentLanguage, builder.WorkContext.CurrentCurrency, builder.WorkContext.QueryString);

            //Initialize product response group.
            //TODO: Need to find possibility to set this response group in theme
            builder.WorkContext.CurrentProductResponseGroup = EnumUtility.SafeParse(builder.WorkContext.QueryString.Get("resp_group"), ItemResponseGroup.ItemMedium | ItemResponseGroup.ItemWithPrices | ItemResponseGroup.ItemWithVendor | ItemResponseGroup.ItemAssociations);

            //This line make delay categories loading initialization (categories can be evaluated on view rendering time)
            builder.WorkContext.Categories = new MutablePagedList<Category>((pageNumber, pageSize, sortInfos, @params) =>
            {
                var criteria = new CategorySearchCriteria(builder.WorkContext.CurrentLanguage)
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    ResponseGroup = CategoryResponseGroup.Small
                };

                if (@params != null)
                {
                    criteria.CopyFrom(@params);
                }
                if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos.IsNullOrEmpty())
                {
                    criteria.SortBy = SortInfo.ToString(sortInfos);
                }
                var result = catalogService.SearchCategories(criteria);
                foreach (var category in result)
                {
                    category.Products = new MutablePagedList<Product>((pageNumber2, pageSize2, sortInfos2, params2) =>
                    {
                        var productSearchCriteria = new ProductSearchCriteria(builder.WorkContext.CurrentLanguage, builder.WorkContext.CurrentCurrency)
                        {
                            PageNumber = pageNumber2,
                            PageSize = pageSize2,
                            Outline = category.Outline,
                            ResponseGroup = builder.WorkContext.CurrentProductSearchCriteria.ResponseGroup
                        };
                        if (params2 != null)
                        {
                            criteria.CopyFrom(params2);
                        }
                        //criteria.CategoryId = category.Id;
                        if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos2.IsNullOrEmpty())
                        {
                            productSearchCriteria.SortBy = SortInfo.ToString(sortInfos2);
                        }

                        return catalogService.SearchProducts(productSearchCriteria).Products;
                    }, 1, ProductSearchCriteria.DefaultPageSize);
                }
                return result;
            }, 1, CategorySearchCriteria.DefaultPageSize);

            //This line make delay products loading initialization (products can be evaluated on view rendering time)
            builder.WorkContext.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos, @params) =>
            {
                var criteria = builder.WorkContext.CurrentProductSearchCriteria.Clone() as ProductSearchCriteria;
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
                var result = catalogService.SearchProducts(criteria);
                //Need change ProductSearchResult with preserve reference because Scriban engine keeps this reference and use new operator will create the new
                //object that doesn't tracked by Scriban
                builder.WorkContext.ProductSearchResult.Aggregations = result.Aggregations;
                builder.WorkContext.ProductSearchResult.Products = result.Products;
                return result.Products;
            }, 1, ProductSearchCriteria.DefaultPageSize);

            builder.WorkContext.ProductSearchResult.Products = builder.WorkContext.Products;

            return Task.CompletedTask;
        }
    }
}
