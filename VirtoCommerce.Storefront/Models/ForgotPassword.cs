using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class ForgotPassword : ValueObject
    {
        public string Email { get; set; }
    }
}
