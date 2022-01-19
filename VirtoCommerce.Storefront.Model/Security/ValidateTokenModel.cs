using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class ValidateTokenModel
    {
        [Required(ErrorMessage = "A Token is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "A Token is required")]
        public string Token { get; set; }
    }
}
