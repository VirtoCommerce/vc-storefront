using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        public bool? RememberBrowser { get; set; }

        public bool? RememberMe { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
