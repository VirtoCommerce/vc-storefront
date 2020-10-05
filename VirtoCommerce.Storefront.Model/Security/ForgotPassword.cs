using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class ForgotPassword : ValueObject
    {
        [Required(ErrorMessage = "An Email or Login is required")]
        [FromForm(Name = "email_login")]
        public string EmailLogin { get; set; }
    }
}
