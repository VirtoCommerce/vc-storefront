using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.DerivativeContractsModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Contracts;
using VirtoCommerce.Storefront.Model.Contracts.Services;

namespace VirtoCommerce.Storefront.Domain.Derivatives
{
    public class DerivativeService : IDerivativeContractService
    {
        private readonly IDerivativeContractOperations _derivativesApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public DerivativeService(IDerivativeContractOperations derivativesApi, IWorkContextAccessor workContextAccessor, IMemoryCache memoryCache, IApiChangesWatcher changesWatcher)
        {
            _derivativesApi = derivativesApi;
            _workContextAccessor = workContextAccessor;
            _memoryCache = memoryCache;
            _apiChangesWatcher = changesWatcher;
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
            var cacheKey = CacheKey.With(GetType(), nameof(EvaluateProductDerivativeInfoAsync), productIds.GetOrderIndependentHashCode().ToString());
            var derivativeContractInfos = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                cacheEntry.AddExpirationToken(DerivativesCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                var context = new AutoRestClients.DerivativeContractsModuleApi.Models.DerivativeContractInfoEvaluationContext
                {
                    MemberId = workContext.CurrentUser.ContactId,
                    ProductIds = productIds,
                    OnlyActive = true
                };

                return await _derivativesApi.EvaluatePromotionsAsync(context);
            });

            foreach (var item in products)
            {
                item.DerivativeInfos = derivativeContractInfos.Select(dci => dci.ToDerivativeInfo()).Where(dci => dci.ProductId == item.Id).ToList();
            }
        }

        public async Task<IList<DerivativeContract>> GetDerivativeContractsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetDerivativeContractsAsync), ids.GetOrderIndependentHashCode().ToString());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(DerivativesCacheRegion.CreateChangeToken());
                return (await _derivativesApi.GetByIdsAsync(ids)).Select(x => x.ToDerivativeContract()).ToList();
            });
        }

        public async Task<IList<DerivativeContract>> GetDerivativeContractItemsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetDerivativeContractItemsAsync), ids.GetOrderIndependentHashCode().ToString());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(DerivativesCacheRegion.CreateChangeToken());
                return (await _derivativesApi.GetByIdsAsync(ids)).Select(x => x.ToDerivativeContract()).ToList();
            });
        }

        public async Task<IPagedList<DerivativeContract>> SearchDerivativeContractsAsync(DerivativeContractSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), nameof(SearchDerivativeContractsAsync), criteria.GetHashCode().ToString());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(DerivativesCacheRegion.CreateChangeToken());
                var resultDto = await _derivativesApi.SearchAsync(criteria.ToSearchCriteriaDto(_workContextAccessor.WorkContext));

                var result = resultDto.Results.Select(x => x.ToDerivativeContract()).ToList();
                return new StaticPagedList<DerivativeContract>(result, criteria.PageNumber, criteria.PageSize, resultDto.TotalCount ?? 0);
            });
        }

        public async Task<IPagedList<DerivativeContractItem>> SearchDerivativeContractItemsAsync(DerivativeContractItemSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), nameof(SearchDerivativeContractItemsAsync), criteria.GetHashCode().ToString());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(DerivativesCacheRegion.CreateChangeToken());
                var resultDto = await _derivativesApi.SearchItemsAsync(criteria.ToSearchCriteriaDto(_workContextAccessor.WorkContext));

                var result = resultDto.Results.Select(x => x.ToDerivativeContractItem()).ToList();
                return new StaticPagedList<DerivativeContractItem>(result, criteria.PageNumber, criteria.PageSize, resultDto.TotalCount ?? 0);
            });
        }
    }
}
