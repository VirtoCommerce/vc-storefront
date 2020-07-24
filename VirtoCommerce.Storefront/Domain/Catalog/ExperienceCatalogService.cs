using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Contracts;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;

namespace VirtoCommerce.Storefront.Domain.Catalog
{
    public class ExperienceCatalogService : CatalogService
    {
        private readonly IGraphQLClient _graphQlClient;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ExperienceCatalogService(
            IGraphQLClient graphQlClient,
            IWorkContextAccessor workContextAccessor,

            ICatalogModuleCategories catalogModuleCategories,
            ICatalogModuleProducts catalogModuleProducts,
            ICatalogModuleIndexedSearch catalogModuleIndexedSearch,
            IPricingService pricingService,
            IMemberService memberService,
            ISubscriptionService subscriptionService,
            IInventoryService inventoryService,
            IStorefrontMemoryCache storefrontMemoryCache,
            IApiChangesWatcher changesWatcher,
            IStorefrontUrlBuilder storefrontUrlBuilder)
            : base(
                workContextAccessor,
                catalogModuleCategories,
                catalogModuleProducts,
                catalogModuleIndexedSearch,
                pricingService,
                memberService,
                subscriptionService,
                inventoryService,
                storefrontMemoryCache,
                changesWatcher,
                storefrontUrlBuilder
            )
        {
            _graphQlClient = graphQlClient;
            _workContextAccessor = workContextAccessor;
        }

        public override async Task<Product[]> GetProductsAsync(string[] ids, ItemResponseGroup responseGroup = ItemResponseGroup.None)
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

            return result;
        }

        public override async Task<Category[]> GetCategoriesAsync(string[] ids, CategoryResponseGroup responseGroup = CategoryResponseGroup.Info)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetCategoriesQuery(ids),
            };

            var response = await _graphQlClient.SendQueryAsync<GetCategoriestResponseDto>(request);

            response.ThrowExceptionOnError();

            var categories = response.Data.Categories.Items.ToCategories();

            EstablishLazyDependenciesForCategories(categories);

            return categories;
        }

        public Category[] GetCategories(string[] ids, CategoryResponseGroup responseGroup)
        {
            return GetCategoriesAsync(ids, responseGroup).GetAwaiter().GetResult();
        }

        public Task<CatalogSearchResult> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<Category>> SearchCategoriesAsync(CategorySearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public CatalogSearchResult SearchProducts(ProductSearchCriteria criteria)
        {
            return SearchProductsAsync(criteria).GetAwaiter().GetResult();
        }

        public IPagedList<Category> SearchCategories(CategorySearchCriteria criteria)
        {
            return SearchCategoriesAsync(criteria).GetAwaiter().GetResult();
        }


        protected virtual async Task<Product[]> GetProductsAsync(string[] ids, WorkContext workContext)
        {
            var currentCurrency = workContext.CurrentCurrency;
            var request = new GraphQLRequest
            {
                Query = this.GetProducts(ids, currentCurrency.CultureName, currentCurrency.Code),
            };
            var response = await _graphQlClient.SendQueryAsync<GetProductsResponseDto>(request);

            response.ThrowExceptionOnError();

            var productDtos = response.Data.Products.Items;

            return productDtos.ToProducts(workContext);
        }

        protected override void EstablishLazyDependenciesForCategories(IEnumerable<Category> categories)
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
                    return new StaticPagedList<Category>(GetCategories(catIds), pageNumber, pageSize, catIds.Length);
                }, 1, CategorySearchCriteria.DefaultPageSize);

                // Lazy loading child categories
                category.Categories = new MutablePagedList<Category>((pageNumber, pageSize, sortInfos, @params) =>
                {
                    var categorySearchCriteria = new CategorySearchCriteria
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Outline = "",
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

        protected override void EstablishLazyDependenciesForProducts(IEnumerable<Product> products)
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
