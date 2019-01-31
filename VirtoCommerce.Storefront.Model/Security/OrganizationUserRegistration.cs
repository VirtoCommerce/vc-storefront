using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class OrganizationUserRegistration : UserRegistration
    {
        [Required]
        public string Role { get; set; }

        [FromForm(Name = "customer[budget]")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Budget { get; set; }

        [Required]
        public string OrganizationId { get; set; }
    }
}
