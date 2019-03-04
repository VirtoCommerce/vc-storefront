using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class ResetPasswordByCodeModel
    {
        [Required(ErrorMessage = "An Email is required")]
        [FromForm(Name = "customer[email]")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Code is required")]
        [FromForm(Name = "customer[code]")]
        public string Code { get; set; }
    }
}
