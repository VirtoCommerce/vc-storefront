using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace VirtoCommerce.Storefront.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class StorefrontRouteAttribute : Attribute, IRouteTemplateProvider
    {
        private const string _regexp = "{store}/{language:regex([[a-zA-Z]]{{2}}(-[[a-zA-Z]]{{2}})?)}/";

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
