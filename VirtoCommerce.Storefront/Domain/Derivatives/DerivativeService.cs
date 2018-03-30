using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.DerivativesModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Derivatives;
using VirtoCommerce.Storefront.Model.Derivatives.Services;

namespace VirtoCommerce.Storefront.Domain.Derivatives
{
    public class DerivativeService : IDerivativeService
    {
        private readonly IDerivativeOperations _derivativesApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IMemoryCache _memoryCache;

        public DerivativeService(IDerivativeOperations derivativesApi, IWorkContextAccessor workContextAccessor, IMemoryCache memoryCache)
        {
            _derivativesApi = derivativesApi;
            _workContextAccessor = workContextAccessor;
            _memoryCache = memoryCache;
        }

        public virtual async Task EvaluateProductDerivativeInfoAsync(List<Product> products, WorkContext workContext)
        {
            if (products == null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            if (!workContext.CurrentUser.IsRegisteredUser || string.IsNullOrEmpty(workContext.CurrentUser.ContactId))
            {
                // anonymous user can't have contracts
                return;
            }

            var productIds = products.Select(x => x.Id).ToList();
            var cacheKey = CacheKey.With(GetType(), "EvaluateProductDerivativeInfoAsync", productIds.GetOrderIndependentHashCode().ToString());
            var searchResult = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                cacheEntry.AddExpirationToken(DerivativesCacheRegion.CreateChangeToken());

                var criteria = new AutoRestClients.DerivativesModuleApi.Models.DerivativeSearchCriteria
                {
                    ProductIds = productIds,
                    MemberIds = new[] { workContext.CurrentUser.ContactId },
                    FulfillmentCenterIds = products.Select(x => x.Inventory?.FulfillmentCenterId)
                                                    .Where(x => !string.IsNullOrEmpty(x))
                                                    .Distinct()
                                                    .ToArray()
                };

                return await _derivativesApi.SearchAsync(criteria);
            });

            var mandatoryDerivatives = searchResult.Derivatives.Where(x => x.IsActive == true && new[] { "Forward", "Futures", "CallOption" }.Contains(x.Type)).Select(x => x.Id);
            var optionalDerivatives = searchResult.Derivatives.Where(x => x.IsActive == true && x.Type == "PutOption").Select(x => x.Id);
            foreach (var item in products)
            {
                item.Derivatives = searchResult.Derivatives.Select(x => x.ToDerivative()).ToList();

                var filteredProductItems = searchResult.Items.Where(x => x.ProductId == item.Id && mandatoryDerivatives.Contains(x.DerivativeId)).ToArray();
                if (filteredProductItems.Any())
                {
                    item.MandatoryDerivativeInfo = new DerivativeInfo
                    {
                        ContractSize = filteredProductItems.Sum(x => (decimal)x.ContractSize),
                        PurchasedQuantity = filteredProductItems.Sum(x => (decimal)x.PurchasedQuantity),
                        RemainingQuantity = filteredProductItems.Sum(x => (decimal)x.RemainingQuantity)
                    };
                }

                filteredProductItems = searchResult.Items.Where(x => x.ProductId == item.Id && optionalDerivatives.Contains(x.DerivativeId)).ToArray();
                if (filteredProductItems.Any())
                {
                    item.OptionalDerivativeInfo = new DerivativeInfo
                    {
                        ContractSize = filteredProductItems.Sum(x => (decimal)x.ContractSize),
                        PurchasedQuantity = filteredProductItems.Sum(x => (decimal)x.PurchasedQuantity),
                        RemainingQuantity = filteredProductItems.Sum(x => (decimal)x.RemainingQuantity)
                    };
                }
            }
        }

        public async Task<IList<Derivative>> GetDerivativesAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), "GetDerivativesAsync", ids.GetOrderIndependentHashCode().ToString());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(DerivativesCacheRegion.CreateChangeToken());
                return (await _derivativesApi.GetByIdsAsync(ids)).Select(x => x.ToDerivative()).ToList();
            });
        }

        public async Task<IPagedList<Derivative>> SearchDerivativesAsync(DerivativeSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), "SearchDerivativesAsync", criteria.GetHashCode().ToString());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(DerivativesCacheRegion.CreateChangeToken());
                var resultDto = await _derivativesApi.SearchAsync(criteria.ToSearchCriteriaDto(_workContextAccessor.WorkContext));

                var result = resultDto.Derivatives.Select(x => x.ToDerivative()).ToList();
                return new StaticPagedList<Derivative>(result, criteria.PageNumber, criteria.PageSize, resultDto.TotalDerivativesCount.Value);
            });
        }

        public async Task<IPagedList<DerivativeItem>> SearchDerivativeItemsAsync(DerivativeItemSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), "SearchDerivativeItemsAsync", criteria.GetHashCode().ToString());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(DerivativesCacheRegion.CreateChangeToken());
                var resultDto = await _derivativesApi.SearchAsync(criteria.ToSearchCriteriaDto(_workContextAccessor.WorkContext));
                var result = resultDto.Items.Select(x => x.ToDerivativeItem()).ToList();
                return new StaticPagedList<DerivativeItem>(result, criteria.PageNumber, criteria.PageSize, resultDto.TotalItemsCount.Value);
            });
        }
    }
}
