using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
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
            var workContext = builder.WorkContext;

            //Initialize catalog search criteria
            var productSearchcriteria = new ProductSearchCriteria(workContext.CurrentLanguage, workContext.CurrentCurrency, workContext.QueryString)
            {
                UserGroups = workContext.CurrentUser?.Contact?.UserGroups ?? new List<string>()
            };
            workContext.CurrentProductSearchCriteria = productSearchcriteria;
            //Initialize product response group.
            //TODO: Need to find possibility to set this response group in theme
            workContext.CurrentProductResponseGroup = EnumUtility.SafeParse(workContext.QueryString.Get("resp_group"), ItemResponseGroup.ItemMedium | ItemResponseGroup.ItemWithPrices | ItemResponseGroup.ItemWithVendor | ItemResponseGroup.ItemAssociations);

            //This line make delay categories loading initialization (categories can be evaluated on view rendering time)
            workContext.Categories = new MutablePagedList<Category>((pageNumber, pageSize, sortInfos, @params) =>
            {
                var criteria = new CategorySearchCriteria(workContext.CurrentLanguage)
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
                        var productSearchCriteria = new ProductSearchCriteria(workContext.CurrentLanguage, workContext.CurrentCurrency)
                        {
                            PageNumber = pageNumber2,
                            PageSize = pageSize2,
                            Outline = category.Outline,
                            ResponseGroup = workContext.CurrentProductSearchCriteria.ResponseGroup,
                            UserGroups = workContext.CurrentUser?.Contact?.UserGroups ?? new List<string>()
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

                        var searchResult = catalogService.SearchProducts(productSearchCriteria);

                        //Because catalog search products returns also aggregations we can use it to populate workContext using C# closure
                        //now workContext.Aggregation will be contains preloaded aggregations for current category
                        workContext.Aggregations = new MutablePagedList<Aggregation>(searchResult.Aggregations);
                        return searchResult.Products;
                    }, 1, ProductSearchCriteria.DefaultPageSize);
                }
                return result;
            }, 1, CategorySearchCriteria.DefaultPageSize);

            //This line make delay products loading initialization (products can be evaluated on view rendering time)
            workContext.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos, @params) =>
            {
                var criteria = workContext.CurrentProductSearchCriteria.Clone() as ProductSearchCriteria;
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
                //Prevent double api request for get aggregations
                //Because catalog search products returns also aggregations we can use it to populate workContext using C# closure
                //now workContext.Aggregation will be contains preloaded aggregations for current search criteria
                workContext.Aggregations = new MutablePagedList<Aggregation>(result.Aggregations);
                return result.Products;
            }, 1, ProductSearchCriteria.DefaultPageSize);

            //This line make delay aggregation loading initialization (aggregation can be evaluated on view rendering time)
            workContext.Aggregations = new MutablePagedList<Aggregation>((pageNumber, pageSize, sortInfos, @params) =>
            {
                var criteria = workContext.CurrentProductSearchCriteria.Clone() as ProductSearchCriteria;
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
                //Force to load products and its also populate workContext.Aggregations by preloaded values
                workContext.Products.Slice(pageNumber, pageSize, sortInfos);
                return workContext.Aggregations;
            }, 1, ProductSearchCriteria.DefaultPageSize);

            return Task.CompletedTask;
        }
    }
}
