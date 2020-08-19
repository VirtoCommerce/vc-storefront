using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using PagedList.Core;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Specifications;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Contracts;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Domain.Catalog
{
    public class ExperienceCatalogService : ICatalogService
    {
        private readonly IGraphQLClient _graphQlClient;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ExperienceCatalogService(
            IGraphQLClient graphQlClient,
            IWorkContextAccessor workContextAccessor)
        {
            _graphQlClient = graphQlClient;
            _workContextAccessor = workContextAccessor;
        }

        public virtual async Task<Product[]> GetProductsAsync(string[] ids, ItemResponseGroup responseGroup = ItemResponseGroup.None)
        {
            Product[] result;

            if (ids.IsNullOrEmpty())
            {
                result = Array.Empty<Product>();
            }
            else
            {
                var workContext = _workContextAccessor.WorkContext;

                result = await GetProductsAsync(ids, workContext);

                EstablishLazyDependenciesForProducts(result);
            }

            foreach (var product in result)
            {
                var associations = product.Associations;
                if (associations != null)
                {
                    await LoadAssociations(associations.ToArray());
                }
            }

            return result;
        }

        public virtual async Task<Category[]> GetCategoriesAsync(string[] ids, CategoryResponseGroup responseGroup = CategoryResponseGroup.Info)
        {
            var workContext = _workContextAccessor.WorkContext;
            var request = new GraphQLRequest
            {
                Query = this.GetCategoriesQuery(ids, workContext.CurrentStore.Id, workContext.CurrentUser.Id),
            };

            var response = await _graphQlClient.SendQueryAsync<GetCategoriestResponseDto>(request);

            response.ThrowExceptionOnError();

            var categories = response.Data.Categories.Items.ToCategories(workContext.CurrentStore, workContext.CurrentLanguage);

            EstablishLazyDependenciesForCategories(categories);

            return categories;
        }

        public virtual Category[] GetCategories(string[] ids, CategoryResponseGroup responseGroup = CategoryResponseGroup.Info)
        {
            return GetCategoriesAsync(ids, responseGroup).GetAwaiter().GetResult();
        }

        public virtual async Task<CatalogSearchResult> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;

            /* Convert price term to intermediate language */
            foreach (var priceTerm in criteria.Terms.Where(x => x.Name.EqualsInvariant("price")))
            {
                priceTerm.ConvertTerm(workContext.CurrentCurrency.Code);
            }

            var request = new GraphQLRequest
            {
                Query = this.SearchProducts(
                    criteria,
                    workContext.CurrentLanguage.CultureName,
                    workContext.CurrentCurrency.Code,
                    workContext.Customer.Id,
                    workContext.CurrentStore.Id,
                    workContext.CurrentStore.Catalog),
            };

            var response = await _graphQlClient.SendQueryAsync<GetProductsResponseDto>(request);

            response.ThrowExceptionOnError();

            var products = response.Data.Products.Items.Select(x => x.ToProduct(workContext)).ToArray();
            var productsWithVariation = products.Concat(products.SelectMany(x => x.Variants)).ToArray();

            if (productsWithVariation.Any())
            {
                EstablishLazyDependenciesForProducts(productsWithVariation);
            }

            var aggrIsVisbileSpec = new AggregationIsVisibleSpecification();

            var aggregations = new List<Aggregation>();
            if (response.Data.Products.TermFacets != null)
            {
                aggregations.AddRange(response.Data.Products.TermFacets.Select(x => x.ToAggregation(workContext.CurrentLanguage.CultureName)));
            }

            if (response.Data.Products.RangeFacets != null)
            {
                aggregations.AddRange(response.Data.Products.RangeFacets.Select(x => x.ToAggregation(workContext.CurrentLanguage.CultureName)));
            }

            var searchResult = new CatalogSearchResult(criteria)
            {
                Products = new MutablePagedList<Product>(products, criteria.PageNumber, criteria.PageSize, response.Data.Products.TotalCount ?? 0),
                Aggregations = !aggregations.IsNullOrEmpty() ? aggregations.Where(x => aggrIsVisbileSpec.IsSatisfiedBy(x)).ToArray() : new Aggregation[] { }
            };

            //Post loading initialization of the resulting aggregations
            var aggrContext = new AggregationPostLoadContext
            {
                ProductSearchCriteria = criteria
            };

            var aggrItemCatIds = searchResult.Aggregations.SelectMany(x => x.Items).OfType<CategoryAggregationItem>().Select(x => x.CategoryId).Distinct().ToArray();
            if (aggrItemCatIds.Any())
            {
                aggrContext.CategoryByIdDict = (await GetCategoriesAsync(aggrItemCatIds, CategoryResponseGroup.Info))
                                                            .Distinct().ToDictionary(x => x.Id)
                                                            .WithDefaultValue(null);
            }

            searchResult.Aggregations.Apply(x => x.PostLoadInit(aggrContext));

            var productAssociations = searchResult
                .Products
                .Where(x => x.Associations != null)
                .SelectMany(x => x.Associations)
                .ToArray();

            await LoadAssociations(productAssociations);

            return searchResult;
        }

        private async Task LoadAssociations(ProductAssociation[] productAssociations)
        {
            if (productAssociations.IsNullOrEmpty())
            {
                return;
            }

            var allAssociations = await GetProductsAsync(productAssociations.Select(x => x.Product.Id).ToArray());

            foreach (var association in productAssociations)
            {
                association.Product = allAssociations.FirstOrDefault(x => x.Id == association.Product.Id);

                if (association.Product != null)
                {
                    EstablishLazyDependenciesForProducts(new[] { association.Product });
                }
            }
        }

        public virtual async Task<IPagedList<Category>> SearchCategoriesAsync(CategorySearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;

            var request = new GraphQLRequest
            {
                Query = this.SearchCategories(
                    criteria,
                    workContext.CurrentStore.Id,
                    workContext.CurrentLanguage.CultureName,
                    workContext.CurrentCurrency.Code,
                    workContext.Customer.Id,
                    workContext.CurrentStore.Catalog),
            };

            var response = await _graphQlClient.SendQueryAsync<GetCategoriestResponseDto>(request);

            response.ThrowExceptionOnError();

            var result = new PagedList<Category>(new List<Category>().AsQueryable(), 1, 1);

            if (!response.Data.Categories.Items.IsNullOrEmpty())
            {
                result = new PagedList<Category>(response.Data.Categories.Items.ToCategories(workContext.CurrentStore, workContext.CurrentLanguage).AsQueryable(), criteria.PageNumber, criteria.PageSize);
            }

            EstablishLazyDependenciesForCategories(result.ToArray());

            return result;
        }

        public virtual CatalogSearchResult SearchProducts(ProductSearchCriteria criteria)
        {
            return SearchProductsAsync(criteria).GetAwaiter().GetResult();
        }

        public virtual IPagedList<Category> SearchCategories(CategorySearchCriteria criteria)
        {
            return SearchCategoriesAsync(criteria).GetAwaiter().GetResult();
        }

        protected virtual async Task<Product[]> GetProductsAsync(string[] ids, WorkContext workContext)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetProducts(
                    ids
                    , workContext.CurrentStore.Id
                    , workContext.CurrentUser.Id
                    , workContext.CurrentLanguage.CultureName
                    , workContext.CurrentCurrency.Code),
            };

            var response = await _graphQlClient.SendQueryAsync<GetProductsResponseDto>(request);

            response.ThrowExceptionOnError();

            var productDtos = response.Data.Products.Items;

            return productDtos.ToProducts(workContext);
        }

        protected virtual void EstablishLazyDependenciesForCategories(IEnumerable<Category> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            foreach (var category in categories)
            {
                // Lazy loading parent category
                category.Parents = new MutablePagedList<Category>((pageNumber, pageSize, sortInfos) =>
                {
                    var catIds = new[] { category.ParentId };
                    return new StaticPagedList<Category>(category.ParentId != null ? GetCategories(catIds) : Array.Empty<Category>(), pageNumber, pageSize, catIds.Length);
                }, 1, CategorySearchCriteria.DefaultPageSize);

                // Lazy loading child categories
                category.Categories = new MutablePagedList<Category>((pageNumber, pageSize, sortInfos, @params) =>
                {
                    var categorySearchCriteria = new CategorySearchCriteria
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Outline = "/" + category.Outline,
                    };

                    if (!sortInfos.IsNullOrEmpty())
                    {
                        categorySearchCriteria.SortBy = SortInfo.ToString(sortInfos);
                    }

                    if (@params != null)
                    {
                        categorySearchCriteria.CopyFrom(@params);
                    }

                    var searchResult = SearchCategories(categorySearchCriteria);
                    return searchResult;
                }, 1, CategorySearchCriteria.DefaultPageSize);
            }
        }

        protected virtual void EstablishLazyDependenciesForProducts(IEnumerable<Product> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            foreach (var product in products)
            {
                product.Category = new Lazy<Category>(() => GetCategories(new[] { product.CategoryId }, CategoryResponseGroup.Small).FirstOrDefault());
            }
        }
    }
}
