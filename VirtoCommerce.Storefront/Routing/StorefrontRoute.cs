using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;

namespace VirtoCommerce.Storefront.Routing
{
    public class StorefrontRoute : Route
    {
        private const string _regexp = "";
        public StorefrontRoute(IRouter target, string routeTemplate, IInlineConstraintResolver inlineConstraintResolver)
            : this(target, routeTemplate, null, null, null, inlineConstraintResolver)
        {
        }

        public StorefrontRoute(IRouter target, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : this(target, null, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
        }

        public StorefrontRoute(IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, _regexp + routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
        }

    }
}
