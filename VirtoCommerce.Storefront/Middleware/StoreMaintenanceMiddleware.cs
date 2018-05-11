using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
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
            if (workContext != null && workContext.CurrentStore != null && workContext.CurrentStore.StoreState == StoreStatus.Closed)
            {
                context.Request.Path = "/common/maintenance";
            }

            await _next(context);
        }
    }

}
