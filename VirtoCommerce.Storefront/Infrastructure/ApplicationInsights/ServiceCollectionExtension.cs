using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.Storefront.Infrastructure.ApplicationInsights
{
    public static class ServiceCollectionExtension
    {
        public static void AddApplicationInsightsExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITelemetryInitializer, UserTelemetryInitializer>();
            services.Configure<SnapshotCollectorConfiguration>(configuration.GetSection(nameof(SnapshotCollectorConfiguration)));
            services.AddSingleton<ITelemetryProcessorFactory>(sp => new SnapshotCollectorTelemetryProcessorFactory(sp));
        }
    }
}
