using Microsoft.AspNetCore.Routing;
using System;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Extensions;

namespace VirtoCommerce.Storefront.Routing
{
    /// <summary>
    /// Reads the slug from request path and tried to find object and controller best matched to it
    /// </summary>
    public class SlugRoute : Route
    {
        public SlugRoute(IRouter target, string template, RouteValueDictionary defaults, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, template, defaults, null, null, inlineConstraintResolver)
        {
        }
      
        protected override async Task OnRouteMatched(RouteContext context)
        {
            var seoRouteService = context.HttpContext.RequestServices.GetRequiredService<ISlugRouteService>();
            var storefrontUrlBuilder = context.HttpContext.RequestServices.GetRequiredService<IStorefrontUrlBuilder>();
            var workContext = context.HttpContext.RequestServices.GetRequiredService<IWorkContextAccessor>().WorkContext;

            var path = context.HttpContext.Request.Path.TrimStoreAndLangSegment(workContext.CurrentStore, workContext.CurrentLanguage).ToString().TrimStart('/');

            if (!string.IsNullOrEmpty(path))
            {
                var seoRouteResponse = await seoRouteService.HandleSlugRequestAsync(path, workContext);
                if (seoRouteResponse != null)
                {
                    if (seoRouteResponse.Redirect)
                    {
                        //Redirect via call specific controller method (because usage of Response.Redirect leads to the rendering the main page)
                        context.RouteData.Values["action"] = "InternalRedirect";
                        context.RouteData.Values["controller"] = "Common";
                        context.RouteData.Values["url"] = storefrontUrlBuilder.ToAppAbsolute(seoRouteResponse.RedirectLocation);
                    }
                    else if (seoRouteResponse.RouteData != null)
                    {
                        foreach (var kvp in seoRouteResponse.RouteData)
                        {
                            context.RouteData.Values[kvp.Key] = kvp.Value;
                        }
                    }
                }
            }
            await base.OnRouteMatched(context);
        }
    }
}
