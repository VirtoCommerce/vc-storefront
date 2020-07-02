using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class ValidationErrorDto : ValidationError
    {
        public void SetErrorCode(string errorCode)
            => this.ErrorCode = errorCode;
    }
}
