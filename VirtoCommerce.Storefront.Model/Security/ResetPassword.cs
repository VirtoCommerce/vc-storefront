using System;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class ResetPassword
    {
        [FromForm(Name = "customer[email]")]
        public string Email { get; set; }

        [FromForm(Name = "customer[username]")]
        public string UserName { get; set; }

        [FromForm(Name = "customer[token]")]
        public string Token { get; set; }
        [FromForm(Name = "customer[password]")]
        public string Password { get; set; }
        [FromForm(Name = "customer[password_confirmation]")]
        public string PasswordConfirmation { get; set; }
    }
}
