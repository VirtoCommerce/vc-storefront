using System;

namespace VirtoCommerce.Storefront.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class StorefrontApiRouteAttribute : StorefrontRouteAttribute
    {
        public StorefrontApiRouteAttribute()
            : this(string.Empty)
        {
        }

        public StorefrontApiRouteAttribute(string template)
            : base($"storefrontapi/{template}")
        {
        }
    }
}
