using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;

namespace VirtoCommerce.Storefront.Model.Cart.Services
{
    public interface IValidatable
    {
        bool IsValid { get; }
        IList<ValidationError> ValidationErrors { get; }
    }
}
