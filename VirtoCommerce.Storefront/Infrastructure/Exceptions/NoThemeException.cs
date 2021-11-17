using System;
using System.Runtime.Serialization;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Infrastructure
{
    [Serializable]
    public class NoThemeException : StorefrontException
    {
        protected NoThemeException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {

        }
        public NoThemeException() : base("No theme defined", "NoTheme")
        {
        }
    }
}
