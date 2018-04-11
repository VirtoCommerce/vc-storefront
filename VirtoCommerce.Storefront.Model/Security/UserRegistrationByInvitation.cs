using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class UserRegistrationByInvitation : UserRegistration
    {
        [Required]
        public string Token { get; set; }
        public string OrganizationId { get; set; }
    }
}
