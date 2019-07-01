using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class UpdatePhoneNumberModel
    {
        [Required]
        [FromForm(Name = "customer[phoneNumber]")]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
