using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class NoStoresException : StorefrontException
    {
        public NoStoresException() : base("No stores defined", "NoStore")
        {
        }
    }
}
