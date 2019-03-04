using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class ForgotPassword : ValueObject
    {
        [Required(ErrorMessage = "A Email is required")]
        public string Email { get; set; }
    }
}
