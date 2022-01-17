using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "A Token is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "A Token is required")]
        public string Token { get; set; }

        [Required(ErrorMessage = "A Password is required")]
        public string Password { get; set; }
    }
}
