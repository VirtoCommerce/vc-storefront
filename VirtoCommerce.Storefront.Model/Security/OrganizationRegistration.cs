using System;
using System.Linq;
using VirtoCommerce.Storefront.Model.Customer;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class OrganizationRegistration : UserRegistration
    {
        public Organization Organization { get; set; } = new Organization();

        public string OrganizationName
        {
            get => Organization.Name;
            set => Organization.Name = value;
        }

        public override Address Address
        {
            get => Organization.Addresses.FirstOrDefault();
            set
            {
                if (value != null)
                {
                    Organization.Addresses.Add(value);
                }
            }
        }
    }
}
