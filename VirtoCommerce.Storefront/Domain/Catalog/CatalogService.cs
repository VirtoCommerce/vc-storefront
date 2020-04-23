using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Specifications;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using catalogDto = VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public class CatalogService : ICatalogService
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IStorefrontUrlBuilder _storefrontUrlBuilder;
        private readonly ICatalogModuleCategories _categoriesApi;
        private readonly ICatalogModuleProducts _productsApi;
        private readonly ICatalogModuleIndexedSearch _searchApi;
        private readonly IPricingService _pricingService;
        private readonly IMemberService _customerService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IInventoryService _inventoryService;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public CatalogService(IWorkContextAccessor workContextAccessor
            , ICatalogModuleCategories categoriesApi
            , ICatalogModuleProducts productsApi
            , ICatalogModuleIndexedSearch searchApi
            , IPricingService pricingService
            , IMemberService customerService
            , ISubscriptionService subscriptionService
            , IInventoryService inventoryService
            , IStorefrontMemoryCache memoryCache
            , IApiChangesWatcher changesWatcher
            , IStorefrontUrlBuilder storefrontUrlBuilder)
        {
            _workContextAccessor = workContextAccessor;
            _categoriesApi = categoriesApi;
            _productsApi = productsApi;
            _searchApi = searchApi;
            _categoriesApi = categoriesApi;
            _pricingService = pricingService;
            _inventoryService = inventoryService;
            _customerService = customerService;
            _subscriptionService = subscriptionService;
            _memoryCache = memoryCache;
            _apiChangesWatcher = changesWatcher;
            _storefrontUrlBuilder = storefrontUrlBuilder;
        }

        #region ICatalogSearchService Members

        public virtual async Task<Product[]> GetProductsAsync(string[] ids, ItemResponseGroup responseGroup = ItemResponseGroup.None)
        {
            Product[] result;

            if (ids.IsNullOrEmpty())
            {
                result = new Product[0];
            }
            else
            {
                var workContext = _workContextAccessor.WorkContext;

                if (responseGroup == ItemResponseGroup.None)
                {
                    responseGroup = workContext.CurrentProductResponseGroup;
                }

                result = await GetProductsAsync(ids, responseGroup, workContext);

                var productsWithVariations = result.Concat(result.SelectMany(p => p.Variations)).ToList();

                await LoadProductDependencies(productsWithVariations, responseGroup, workContext);
                EstablishLazyDependenciesForProducts(result);
            }

            return result;
        }

        public virtual Category[] GetCategories(string[] ids, CategoryResponseGroup responseGroup = CategoryResponseGroup.Info)
        {
            return GetCategoriesAsync(ids, responseGroup).GetAwaiter().GetResult();
        }

        public virtual async Task<Category[]> GetCategoriesAsync(string[] ids, CategoryResponseGroup responseGroup = CategoryResponseGroup.Info)
        {
            var workContext = _workContextAccessor.WorkContext;
            var cacheKey = CacheKey.With(GetType(), nameof(GetCategoriesAsync), string.Join("-", ids.OrderBy(x => x)), responseGroup.ToString());
            var categoriesDto = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                return await _categoriesApi.GetCategoriesByPlentyIdsAsync(ids.ToList(), ((int)responseGroup).ToString());
            });
            var result = categoriesDto.Select(x => x.ToCategory(workContext.CurrentLanguage, workContext.CurrentStore)).ToArray();
            //Set  lazy loading for child categories 
            EstablishLazyDependenciesForCategories(result);        
            return result;
        }

        /// <summary>
        /// Search categories by given criteria 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual IPagedList<Category> SearchCategories(CategorySearchCriteria criteria)
        {
            return SearchCategoriesAsync(criteria).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Async search categories by given criteria 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual async Task<IPagedList<Category>> SearchCategoriesAsync(CategorySearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            var cacheKey = CacheKey.With(GetType(), nameof(SearchCategoriesAsync), criteria.GetCacheKey(), workContext.CurrentStore.Id, workContext.CurrentLanguage.CultureName, workContext.CurrentCurrency.Code);
            var searchResult = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                criteria = criteria.Clone() as CategorySearchCriteria;
                var searchCriteria = criteria.ToCategorySearchCriteriaDto(workContext);
                return await _searchApi.SearchCategoriesAsync(searchCriteria);


            });
            var result = new PagedList<Category>(new List<Category>().AsQueryable(), 1, 1);
            if (searchResult.Items != null)
            {
                result = new PagedList<Category>(searchResult.Items.Select(x => x.ToCategory(workContext.CurrentLanguage, workContext.CurrentStore)).AsQueryable(), criteria.PageNumber, criteria.PageSize);
            }
            //Set  lazy loading for child categories 
            EstablishLazyDependenciesForCategories(result.ToArray());
            return result;
        }

        /// <summary>
        /// Search products by given criteria 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual CatalogSearchResult SearchProducts(ProductSearchCriteria criteria)
        {
            return SearchProductsAsync(criteria).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Async search products by given criteria 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual async Task<CatalogSearchResult> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;

            var result = await SearchProductsAsync(criteria, workContext);
            var products = result.Items?.Select(x => x.ToProduct(workContext.CurrentLanguage, workContext.CurrentCurrency, workContext.CurrentStore)).ToList() ?? new List<Product>();
            var productsWithVariations = products.Concat(products.SelectMany(p => p.Variations)).ToList();

            if (productsWithVariations.Any())
            {
                await LoadProductDependencies(productsWithVariations, criteria.ResponseGroup, workContext);
                EstablishLazyDependenciesForProducts(productsWithVariations);
            }

            var aggrIsVisbileSpec = new AggregationIsVisibleSpecification();
            var searchResult = new CatalogSearchResult(criteria)
            {
                Products = new MutablePagedList<Product>(products, criteria.PageNumber, criteria.PageSize, (int?)result.TotalCount ?? 0),
                Aggregations = !result.Aggregations.IsNullOrEmpty() ? result.Aggregations.Select(x => x.ToAggregation(workContext.CurrentLanguage.CultureName))
                                                                                         .Where(x => aggrIsVisbileSpec.IsSatisfiedBy(x))                                                                                         
                                                                                         .ToArray() : new Aggregation[] { }
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
            return searchResult;
        }
        #endregion

        protected virtual async Task LoadProductDependencies(List<Product> products, ItemResponseGroup responseGroup, WorkContext workContext)
        {
            if (!products.IsNullOrEmpty())
            {
                var taskList = new List<Task>();

                if (responseGroup.HasFlag(ItemResponseGroup.Inventory))
                {
                    taskList.Add(LoadProductInventoriesAsync(products, workContext));
                }

                if (responseGroup.HasFlag(ItemResponseGroup.ItemAssociations))
                {
                    taskList.Add(LoadProductsAssociationsAsync(products, workContext));
                }

                if (responseGroup.HasFlag(ItemResponseGroup.ItemWithPrices))
                {
                    taskList.Add(_pricingService.EvaluateProductPricesAsync(products, workContext));
                }

                if (responseGroup.HasFlag(ItemResponseGroup.ItemWithVendor))
                {
                    taskList.Add(LoadProductVendorsAsync(products, workContext));
                }

                if (workContext.CurrentStore.SubscriptionEnabled && responseGroup.HasFlag(ItemResponseGroup.ItemWithPaymentPlan))
                {
                    taskList.Add(LoadProductPaymentPlanAsync(products, workContext));
                }

                await Task.WhenAll(taskList.ToArray());

                foreach (var product in products)
                {
                    product.IsBuyable = new ProductIsBuyableSpecification().IsSatisfiedBy(product);
                    product.IsAvailable = new ProductIsAvailableSpecification(product).IsSatisfiedBy(1);
                    product.IsInStock = new ProductIsInStockSpecification().IsSatisfiedBy(product);
                }
            }
        }

        protected virtual async Task<Product[]> GetProductsAsync(IList<string> ids, ItemResponseGroup responseGroup, WorkContext workContext)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetProductsAsync), string.Join("-", ids.OrderBy(x => x)), responseGroup.ToString());
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                return await _productsApi.GetProductByPlentyIdsAsync(ids, ((int)responseGroup).ToString());
            });
            return result.Select(x => x.ToProduct(workContext.CurrentLanguage, workContext.CurrentCurrency, workContext.CurrentStore)).ToArray();
        }

        protected virtual async Task<catalogDto.ProductIndexedSearchResult> SearchProductsAsync(ProductSearchCriteria criteria, WorkContext workContext)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(SearchProductsAsync), criteria.GetCacheKey(), workContext.CurrentStore.Id, workContext.CurrentLanguage.CultureName, workContext.CurrentCurrency.Code);

            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                criteria = criteria.Clone() as ProductSearchCriteria;

                var searchCriteria = criteria.ToProductSearchCriteriaDto(workContext);
                return await _searchApi.SearchProductsAsync(searchCriteria);
            });
        }

        protected virtual async Task LoadProductVendorsAsync(List<Product> products, WorkContext workContext)
        {
            var vendorIds = products.Where(p => !string.IsNullOrEmpty(p.VendorId)).Select(p => p.VendorId).Distinct().ToArray();
            if (!vendorIds.IsNullOrEmpty())
            {
                var vendors = await _customerService.GetVendorsByIdsAsync(workContext.CurrentStore, workContext.CurrentLanguage, vendorIds);
                foreach (var product in products)
                {
                    product.Vendor = vendors.FirstOrDefault(v => v != null && v.Id == product.VendorId);
                    if (product.Vendor != null)
                    {
                        product.Vendor.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos, @params) =>
                        {
                            var criteria = new ProductSearchCriteria
                            {
                                VendorId = product.VendorId,
                                PageNumber = pageNumber,
                                PageSize = pageSize,
                                ResponseGroup = workContext.CurrentProductSearchCriteria.ResponseGroup & ~ItemResponseGroup.ItemWithVendor,
                                SortBy = SortInfo.ToString(sortInfos),
                            };
                            if (@params != null)
                            {
                                criteria.CopyFrom(@params);
                            }
                            var searchResult = SearchProducts(criteria);
                            return searchResult.Products;
                        }, 1, ProductSearchCriteria.DefaultPageSize);
                    }
                }
            }
        }

      
        protected virtual async Task LoadProductInventoriesAsync(List<Product> products, WorkContext workContext)
        {
            await _inventoryService.EvaluateProductInventoriesAsync(products, workContext);
        }

        protected virtual async Task LoadProductPaymentPlanAsync(List<Product> products, WorkContext workContext)
        {
            var paymentPlans = await _subscriptionService.GetPaymentPlansByIdsAsync(products.Select(x => x.Id).ToArray());
            foreach (var product in products)
            {
                product.PaymentPlan = paymentPlans.FirstOrDefault(x => product.Equals(x));
            }
        }

        protected virtual Task LoadProductsAssociationsAsync(IEnumerable<Product> products, WorkContext workContext)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            foreach (var product in products)
            {
                //Associations 
                product.Associations = new MutablePagedList<ProductAssociation>((pageNumber, pageSize, sortInfos, @params) =>
                {
                    var criteria = new ProductAssociationSearchCriteria
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        ProductId = product.Id,
                        ResponseGroup = ItemResponseGroup.ItemInfo | ItemResponseGroup.ItemWithPrices | ItemResponseGroup.Inventory | ItemResponseGroup.ItemWithVendor
                    };
                    if (!sortInfos.IsNullOrEmpty())
                    {
                        criteria.Sort = SortInfo.ToString(sortInfos);
                    }
                    if (@params != null)
                    {
                        criteria.CopyFrom(@params);
                    }
                    var cacheKey = CacheKey.With(GetType(), nameof(LoadProductsAssociationsAsync), criteria.GetCacheKey());
                    var searchResult = _memoryCache.GetOrCreateExclusive(cacheKey, cacheEntry =>
                       {
                           cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                           cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());
                           return _productsApi.SearchProductAssociations(criteria.ToProductAssociationSearchCriteriaDto());
                       });
                    //Load products for resulting associations
                    var associatedProducts = GetProductsAsync(searchResult.Results.Select(x => x.AssociatedObjectId).ToArray(), criteria.ResponseGroup).GetAwaiter().GetResult();
                    var result = new List<ProductAssociation>();
                    foreach (var associationDto in searchResult.Results)
                    {
                        var productAssociation = associationDto.ToProductAssociation();
                        productAssociation.Product = associatedProducts.FirstOrDefault(x => x.Id.EqualsInvariant(productAssociation.ProductId));
                        result.Add(productAssociation);
                    }
                    return new StaticPagedList<ProductAssociation>(result, pageNumber, pageSize, searchResult.TotalCount ?? 0);
                }, 1, ProductSearchCriteria.DefaultPageSize);
            }
            return Task.CompletedTask;
        }

        protected virtual void EstablishLazyDependenciesForCategories(IEnumerable<Category> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            foreach (var category in categories)
            {
                //Lazy loading for parents categories
                category.Parents = new MutablePagedList<Category>((pageNumber, pageSize, sortInfos) =>
                {
                    var catIds = category.Outline.Split('/').Where(x =>  x != null && !x.EqualsInvariant(category.Id)).ToArray();
                    return new StaticPagedList<Category>(GetCategories(catIds, CategoryResponseGroup.Small), pageNumber, pageSize, catIds.Length);
                }, 1, CategorySearchCriteria.DefaultPageSize);

                //Lazy loading for child categories
                category.Categories = new MutablePagedList<Category>((pageNumber, pageSize, sortInfos, @params) =>
                {
                    var categorySearchCriteria = new CategorySearchCriteria
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Outline = "/" + category.Outline
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
            foreach (var product in products.Where(x => !string.IsNullOrEmpty(x.CategoryId)))
            {
                product.Category = new Lazy<Category>(() => GetCategories(new[] { product.CategoryId }, CategoryResponseGroup.Small).FirstOrDefault());
            }            
        }
    }
}
