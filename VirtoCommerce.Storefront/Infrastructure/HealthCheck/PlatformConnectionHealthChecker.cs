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
                await _storeService.GetAllStoresAsync();
                return HealthCheckResult.Healthy("Connection to the Platform is OK");
            }
            catch
            {
                return HealthCheckResult.Unhealthy("Connection to the Platform fails");
            }

        }
    }
}
