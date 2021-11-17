using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Models;

namespace VirtoCommerce.Storefront.Middleware
{
    public class NoLiquidThemeMiddleware
    {
        public const string NoThemeModelKey = "NoLiquidThemeMiddleware.NoThemeModel";
        private readonly RequestDelegate _next;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ILiquidThemeEngine _liquidThemeEngine;
        public NoLiquidThemeMiddleware(RequestDelegate next, IWorkContextAccessor workContextAccessor, ILiquidThemeEngine liquidThemeEngine)
        {
            _next = next;
            _workContextAccessor = workContextAccessor;
            _liquidThemeEngine = liquidThemeEngine;
        }

        public async Task Invoke(HttpContext context)
        {
            var workContext = _workContextAccessor.WorkContext;
            if (workContext != null && string.IsNullOrEmpty(_liquidThemeEngine.ResolveTemplatePath("index")))
            {
                context.Request.Path = "/common/notheme";
                context.Items[NoThemeModelKey] = new NoThemeViewModel { SearchedLocations = _liquidThemeEngine.DiscoveryPaths.ToList() };
            }

            await _next(context);
        }
    }

}
