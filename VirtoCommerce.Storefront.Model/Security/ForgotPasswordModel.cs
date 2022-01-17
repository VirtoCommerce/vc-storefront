using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "A Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "URL is required")]
        public string ResetPasswordUrl { get; set; }
    }
}
