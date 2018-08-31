using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.Storefront.Routing
{
    /// <summary>
    /// Provides extension methods for <see cref="IRouteBuilder"/> to add routes.
    /// </summary>
    public static class MapStorefrontRouteBuilderExtension
    {
        public static IRouteBuilder MapStorefrontRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string template)
        {
            return MapStorefrontRoute(routeBuilder, name, template, defaults: null);
        }

        public static IRouteBuilder MapStorefrontRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string template,
            object defaults)
        {
            return MapStorefrontRoute(routeBuilder, name, template, defaults, constraints: null);
        }


        public static IRouteBuilder MapStorefrontRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string template,
            object defaults,
            object constraints)
        {
            return MapStorefrontRoute(routeBuilder, name, template, defaults, constraints, dataTokens: null);
        }


        public static IRouteBuilder MapStorefrontRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string template,
            object defaults,
            object constraints,
            object dataTokens)
        {
            if (routeBuilder.DefaultHandler == null)
            {
                throw new RouteCreationException($"Must be set { nameof(IRouteBuilder) }");
            }

            var inlineConstraintResolver = routeBuilder
                .ServiceProvider
                .GetRequiredService<IInlineConstraintResolver>();

            routeBuilder.Routes.Add(new StorefrontRoute(
                routeBuilder.DefaultHandler,
                name,
                template,
                new RouteValueDictionary(defaults),
                new RouteValueDictionary(constraints),
                new RouteValueDictionary(dataTokens),
                inlineConstraintResolver));

            return routeBuilder;
        }

        public static IRouteBuilder MapSlugRoute(
           this IRouteBuilder routeBuilder,
           string template,
           object defaults)
        {
            if (routeBuilder.DefaultHandler == null)
            {
                throw new RouteCreationException($"Must be set { nameof(IRouteBuilder) }");
            }

            var inlineConstraintResolver = routeBuilder
                .ServiceProvider
                .GetRequiredService<IInlineConstraintResolver>();

            routeBuilder.Routes.Add(new SlugRoute(
                routeBuilder.DefaultHandler,
                template,
                new RouteValueDictionary(defaults),
                inlineConstraintResolver));

            return routeBuilder;
        }
    }
}
