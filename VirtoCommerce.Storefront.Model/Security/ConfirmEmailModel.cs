using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class ConfirmEmailModel
    {
        [Required(ErrorMessage = "A UserId is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "A Token is required")]
        public string Token { get; set; }
    }
}
