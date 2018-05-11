using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class OrganizationRegistration : UserRegistration
    {
        [Required]
        public string OrganizationName { get; set; }
    }
}
