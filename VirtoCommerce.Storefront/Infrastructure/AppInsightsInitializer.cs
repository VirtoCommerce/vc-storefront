using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class AppInsightsInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppInsightsInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Initialize(ITelemetry telemetry)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.User.Identity.IsAuthenticated && httpContext.User.Identity.Name != null)
            {
                telemetry.Context.User.AuthenticatedUserId = httpContext.User.Identity.Name;
                telemetry.Context.User.AccountId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
        }
    }
}
