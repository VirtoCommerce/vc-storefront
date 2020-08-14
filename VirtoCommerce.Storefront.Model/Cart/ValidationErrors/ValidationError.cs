using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Cart.ValidationErrors
{
    public abstract class ValidationError : ValueObject
    {
        protected ValidationError()
        {
            ErrorCode = GetType().Name;
        }

        public string ErrorCode { get; set; }
        public string ObjectType { get; set; }
        public string ObjectId { get; set; }
        public string ErrorMessage { get; set; }
        public List<ErrorParameter> ErrorParameters { get; set; }
    }
}
