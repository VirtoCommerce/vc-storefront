using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Data.Stores;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Middleware
{
    public class StoreMaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWorkContextAccessor _workContextAccessor;
        public StoreMaintenanceMiddleware(RequestDelegate next, IWorkContextAccessor workContextAccessor)
        {
            _next = next;
            _workContextAccessor = workContextAccessor;
        }

        public async Task Invoke(HttpContext context)
        {
            var workContext = _workContextAccessor.WorkContext;
            if (workContext != null && workContext.CurrentStore.StoreState == StoreStatus.Closed)
            {
                context.Request.Path = "/common/maintenance";
            }

            await _next(context);
        }
    }

}
