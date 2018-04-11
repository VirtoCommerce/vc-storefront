using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class InviteRegister
    {
        [FromForm(Name = "customer[email]")]
        public string Email { get; set; }
        [FromForm(Name = "customer[token]")]

        public string Token { get; set; }
        [FromForm(Name = "customer[first_name]")]
        public string FirstName { get; set; }
        [FromForm(Name = "customer[last_name]")]
        public string LastName { get; set; }
        [FromForm(Name = "customer[user_name]")]
        public string UserName { get; set; }

        [FromForm(Name = "customer[password]")]
        public string Password { get; set; }
        [FromForm(Name = "customer[password_confirmation]")]
        public string PasswordConfirmation { get; set; }
    }
}
