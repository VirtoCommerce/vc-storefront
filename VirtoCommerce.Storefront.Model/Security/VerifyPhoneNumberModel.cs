using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class VerifyPhoneNumberModel
    {
        [Required]
        [FromForm(Name = "customer[code]")]
        public string Code { get; set; }

        [Required]
        [FromForm(Name = "customer[phoneNumber]")]
        public string PhoneNumber { get; set; }
    }
}
