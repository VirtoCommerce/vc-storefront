using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{

    public partial class Login 
    {
        
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [FromForm(Name = "customer[user_name]")]
        public string Username { get; set; }
        [FromForm(Name = "customer[password]")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}
