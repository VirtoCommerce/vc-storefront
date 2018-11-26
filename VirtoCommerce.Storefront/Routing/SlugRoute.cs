using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
            var serviceProvider = context.HttpContext.RequestServices;

            var seoRouteService = serviceProvider.GetRequiredService<ISlugRouteService>();
            var storefrontUrlBuilder = serviceProvider.GetRequiredService<IStorefrontUrlBuilder>();
            var workContext = serviceProvider.GetRequiredService<IWorkContextAccessor>().WorkContext;

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

                    var actionDescriptor = FindMatchingActionDescriptor(context.RouteData.Values, serviceProvider);
                    if (actionDescriptor != null)
                    {
                        context.Handler = httpContext =>
                        {
                            var actionContext = new ActionContext(httpContext, context.RouteData, actionDescriptor);

                            var actionInvokerFactory = serviceProvider.GetRequiredService<IActionInvokerFactory>();
                            var invoker = actionInvokerFactory.CreateInvoker(actionContext);

                            return invoker.InvokeAsync();
                        };

                        return;
                    }
                }
            }
            await base.OnRouteMatched(context);
        }

        private ActionDescriptor FindMatchingActionDescriptor(IReadOnlyDictionary<string, object> routeDataValues,
            IServiceProvider serviceProvider)
        {
            if (!routeDataValues.TryGetValue("controller", out var controllerNameObject))
            {
                return null;
            }

            if (!routeDataValues.TryGetValue("action", out var actionNameObject))
            {
                return null;
            }

            var controllerName = (string)controllerNameObject;
            var actionName = (string)actionNameObject;

            var actionDescriptorCollectionProvider = serviceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();
            var actions = actionDescriptorCollectionProvider.ActionDescriptors;

            var controllerActions = actions.Items.OfType<ControllerActionDescriptor>();

            var matchingAction = controllerActions.FirstOrDefault(action =>
                action.ControllerName == controllerName &&
                action.ActionName == actionName);

            return matchingAction;
        }
    }
}
