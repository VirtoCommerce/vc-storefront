using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Customer
{
    public partial class ContactUpdateInfo : ValueObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
