using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace VirtoCommerce.Storefront.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class StorefrontRouteAttribute : Attribute, IRouteTemplateProvider
    {
        private const string _regexp = "";

        public StorefrontRouteAttribute()
            : this(string.Empty)
        {
        }

        public StorefrontRouteAttribute(string template)
        {
            Template = _regexp + template;
        }

        #region IRouteTemplateProvider members
        public string Template
        {
            get;
            private set;
        }

        public int? Order => 0;

        public string Name => null;
        #endregion
    }
}
