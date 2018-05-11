using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class OrganizationUserRegistration : UserRegistration
    {
        [Required]
        public string Role { get; set; }
        [Required]
        public string OrganizationId { get; set; }
    }
}
