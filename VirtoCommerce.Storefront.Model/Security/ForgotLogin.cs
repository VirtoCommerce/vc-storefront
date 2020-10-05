using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class ForgotLogin : ValueObject
    {
        [Required(ErrorMessage = "An Email is required")]
        public string Email { get; set; }
    }
}
