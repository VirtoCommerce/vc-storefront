using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Cart.ValidationErrors
{
    public abstract class ValidationError : ValueObject
    {
        protected ValidationError()
        {
            ErrorCode = GetType().Name;
        }

        public string ErrorCode { get; private set; }
    }
}
