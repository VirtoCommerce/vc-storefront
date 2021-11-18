using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{

    public partial class Login
    {
        [EmailAddress]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [FromForm(Name = "customer[user_name]")]
        [Required]
        public string UserName { get; set; }
        [FromForm(Name = "customer[password]")]
        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public bool DisplayCaptcha { get; set; }

        public bool ForceLoginToAccountStore { get; set; }
    }
}
