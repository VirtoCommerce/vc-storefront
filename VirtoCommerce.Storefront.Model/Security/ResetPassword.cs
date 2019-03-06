using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class ResetPassword
    {
        [FromForm(Name = "customer[email]")]
        public string Email { get; set; }

        [FromForm(Name = "customer[user_name]")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "A Token is required")]
        [FromForm(Name = "customer[token]")]
        public string Token { get; set; }

        [Required(ErrorMessage = "A Password is required")]
        [FromForm(Name = "customer[password]")]
        public string Password { get; set; }

        [Required(ErrorMessage = "A Password confirmation is required")]
        [FromForm(Name = "customer[password_confirmation]")]
        public string PasswordConfirmation { get; set; }
    }
}
