using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.Common.Exceptions;
using VirtoCommerce.Storefront.Data.Stores;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Stores;
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
                context.Items[NoThemeModelKey] = new NoThemeViewModel { SearchedLocations = _liquidThemeEngine.DiscoveryPaths.Select(x => Path.GetDirectoryName(x)) };
            }

            await _next(context);
        }
    }

}
