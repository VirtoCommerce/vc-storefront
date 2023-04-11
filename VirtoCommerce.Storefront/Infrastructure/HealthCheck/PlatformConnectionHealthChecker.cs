using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Infrastructure.HealthCheck
{
    public class PlatformConnectionHealthChecker : IHealthCheck
    {
        private readonly IStoreService _storeService;

        public PlatformConnectionHealthChecker(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var stores = await _storeService.GetAllStoresAsync();
                //if (stores.IsNullOrEmpty())
                //{
                //    return HealthCheckResult.
                //}
                return HealthCheckResult.Healthy("Connection to the Platform is OK");
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy("Connection to the Platform fails");
            }

        }
    }
}
