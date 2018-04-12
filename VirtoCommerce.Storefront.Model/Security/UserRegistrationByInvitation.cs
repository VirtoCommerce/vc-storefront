using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class UserRegistrationByInvitation : UserRegistration
    {
        [Required]
        [FromForm(Name = "customer[token]")]
        public string Token { get; set; }
        [FromForm(Name = "customer[organization_id]")]
        public string OrganizationId { get; set; }
    }
}
