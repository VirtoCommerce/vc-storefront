using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class OrganizationRegistration : UserRegistration
    {
        
        public string OrganizationName { get; set; }
    }
}
