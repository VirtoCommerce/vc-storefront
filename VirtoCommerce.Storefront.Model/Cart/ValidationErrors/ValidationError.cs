using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Cart.ValidationErrors
{
    public abstract class ValidationError : ValueObject
    {
        public ValidationError()
        {
            ErrorCode = GetType().Name;
        }

        public string ErrorCode { get; private set; }
    }
}
