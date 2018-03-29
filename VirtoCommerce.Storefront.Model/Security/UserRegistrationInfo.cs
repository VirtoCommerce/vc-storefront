using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class UserRegistrationInfo : ValueObject
    {
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string StoreId { get; set; }
        public string Role { get; set; }
        public string NewOrganizationName { get; set; }
        public string ExistOrganizationId { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
    }
}
