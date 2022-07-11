using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{

    public partial class Login
    {
        [EmailAddress]
        public string Email { get; set; }


        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
