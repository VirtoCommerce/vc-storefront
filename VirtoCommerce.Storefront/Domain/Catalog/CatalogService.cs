using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi;
using VirtoCommerce.Storefront.Caching;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Inventory.Services;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class CatalogService : ICatalogService
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICatalogModuleCategories _categoriesApi;
        private readonly ICatalogModuleProducts _productsApi;
        private readonly ICatalogModuleSearch _searchApi;
        private readonly IPricingService _pricingService;
        private readonly IMemberService _customerService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IInventoryService _inventoryService;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public CatalogService(IWorkContextAccessor workContextAccessor, ICatalogModuleCategories categoriesApi,
            ICatalogModuleProducts productsApi,
            ICatalogModuleSearch searchApi, IPricingService pricingService, IMemberService customerService,
            ISubscriptionService subscriptionService,
            IInventoryService inventoryService, IStorefrontMemoryCache memoryCache, IApiChangesWatcher changesWatcher)
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

                var allProducts = result.Concat(result.SelectMany(p => p.Variations)).ToList();


                if (!allProducts.IsNullOrEmpty())
                {
                    var taskList = new List<Task>();

                    if (responseGroup.HasFlag(ItemResponseGroup.Inventory))
                    {
                        taskList.Add(LoadProductInventoriesAsync(allProducts, workContext));
                    }

                    if (responseGroup.HasFlag(ItemResponseGroup.ItemAssociations))
                    {
                        taskList.Add(LoadProductsAssociationsAsync(allProducts, workContext));
                    }

                    if (responseGroup.HasFlag(ItemResponseGroup.ItemWithPrices))
                    {
                        taskList.Add(_pricingService.EvaluateProductPricesAsync(allProducts, workContext));
                    }

                    if (responseGroup.HasFlag(ItemResponseGroup.ItemWithVendor))
                    {
                        taskList.Add(LoadProductVendorsAsync(allProducts, workContext));
                    }

                    if (workContext.CurrentStore.SubscriptionEnabled && responseGroup.HasFlag(ItemResponseGroup.ItemWithPaymentPlan))
                    {
                        taskList.Add(LoadProductPaymentPlanAsync(allProducts, workContext));
                    }

                    await Task.WhenAll(taskList.ToArray());

                    foreach (var product in allProducts)
                    {
                        product.IsBuyable = new ProductIsBuyableSpecification().IsSatisfiedBy(product);
                        product.IsAvailable = new ProductIsAvailableSpecification(product).IsSatisfiedBy(1);
                        product.IsInStock = new ProductIsInStockSpecification().IsSatisfiedBy(product);
                    }
                }
            }

            return result;
        }

        public virtual Category[] GetCategories(string[] ids, CategoryResponseGroup responseGroup = CategoryResponseGroup.Info)
        {
            var workContext = _workContextAccessor.WorkContext;
            return GetCategoriesAsync(ids, responseGroup).GetAwaiter().GetResult();
        }

        public virtual async Task<Category[]> GetCategoriesAsync(string[] ids, CategoryResponseGroup responseGroup = CategoryResponseGroup.Info)
        {
            var workContext = _workContextAccessor.WorkContext;
            var cacheKey = CacheKey.With(GetType(), "GetCategoriesAsync", string.Join("-", ids.OrderBy(x => x)), responseGroup.ToString());
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
            var workContext = _workContextAccessor.WorkContext;
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
            var cacheKey = CacheKey.With(GetType(), "SearchCategoriesAsync", criteria.GetCacheKey(), workContext.CurrentStore.Id, workContext.CurrentLanguage.CultureName, workContext.CurrentCurrency.Code);
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
            var cacheKey = CacheKey.With(GetType(), "SearchProductsAsync", criteria.GetCacheKey(), workContext.CurrentStore.Id, workContext.CurrentLanguage.CultureName, workContext.CurrentCurrency.Code);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                criteria = criteria.Clone() as ProductSearchCriteria;

                var searchCriteria = criteria.ToProductSearchCriteriaDto(workContext);
                var result = await _searchApi.SearchProductsAsync(searchCriteria);
                var products = result.Items?.Select(x => x.ToProduct(workContext.CurrentLanguage, workContext.CurrentCurrency, workContext.CurrentStore)).ToList() ?? new List<Product>();

                if (products.Any())
                {
                    var productsWithVariations = products.Concat(products.SelectMany(x => x.Variations)).ToList();
                    var taskList = new List<Task>();

                    if (criteria.ResponseGroup.HasFlag(ItemResponseGroup.Inventory))
                    {
                        taskList.Add(LoadProductInventoriesAsync(productsWithVariations, workContext));
                    }

                    if (criteria.ResponseGroup.HasFlag(ItemResponseGroup.ItemAssociations))
                    {
                        taskList.Add(LoadProductsAssociationsAsync(productsWithVariations, workContext));
                    }

                    if (criteria.ResponseGroup.HasFlag(ItemResponseGroup.ItemWithVendor))
                    {
                        taskList.Add(LoadProductVendorsAsync(productsWithVariations, workContext));
                    }

                    if (criteria.ResponseGroup.HasFlag(ItemResponseGroup.ItemWithPrices))
                    {
                        taskList.Add(_pricingService.EvaluateProductPricesAsync(productsWithVariations, workContext));
                    }

                    await Task.WhenAll(taskList.ToArray());

                    foreach (var product in productsWithVariations)
                    {
                        product.IsBuyable = new ProductIsBuyableSpecification().IsSatisfiedBy(product);
                        product.IsAvailable = new ProductIsAvailableSpecification(product).IsSatisfiedBy(1);
                        product.IsInStock = new ProductIsInStockSpecification().IsSatisfiedBy(product);
                    }
                }
                return new CatalogSearchResult
                {
                    Products = new StaticPagedList<Product>(products, criteria.PageNumber, criteria.PageSize, (int?)result.TotalCount ?? 0),
                    Aggregations = !result.Aggregations.IsNullOrEmpty() ? result.Aggregations.Select(x => x.ToAggregation(workContext.CurrentLanguage.CultureName)).ToArray() : new Aggregation[] { }
                };
            });
        }
        #endregion


        protected virtual async Task<Product[]> GetProductsAsync(IList<string> ids, ItemResponseGroup responseGroup, WorkContext workContext)
        {
            var cacheKey = CacheKey.With(GetType(), "GetProductsAsync", string.Join("-", ids.OrderBy(x => x)), responseGroup.ToString());
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CatalogCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                return await _productsApi.GetProductByPlentyIdsAsync(ids, ((int)responseGroup).ToString());
            });
            return result.Select(x => x.ToProduct(workContext.CurrentLanguage, workContext.CurrentCurrency, workContext.CurrentStore)).ToArray();
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
                    var cacheKey = CacheKey.With(GetType(), "SearchProductAssociations", criteria.GetCacheKey());
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
                    var catIds = category.Outline.Split('/');
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

    }
}
