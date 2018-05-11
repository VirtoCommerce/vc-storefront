using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class StorefrontRouteAttribute : Attribute, IRouteTemplateProvider
    {
        private const string _regexp = "{store}/{language:regex([[a-zA-Z]]{{2}}-[[a-zA-Z]]{{2}})}/";
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
