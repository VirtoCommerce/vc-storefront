using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class NoThemeException : StorefrontException
    {
        public NoThemeException() : base("No theme defined", "NoTheme")
        {
        }
    }
}
