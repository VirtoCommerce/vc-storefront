using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.Storefront.Infrastructure.ApplicationInsights
{
    public static class ServiceCollectionExtension
    {
        public static void AddApplicationInsightsExtensions(this IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer, UserTelemetryInitializer>();
        }
    }
}
