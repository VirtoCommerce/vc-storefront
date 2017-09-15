using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class ResetPassword : ValueObject<ForgotPassword>
    {
        [FromForm(Name = "customer[password]")]
        public string Password { get; set; }
        [FromForm(Name = "customer[password_confirmation]")]
        public string PasswordConfirmation { get; set; }
    }
}
