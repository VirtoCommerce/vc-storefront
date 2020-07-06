using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Customer.Commands
{
    public class CreateOrganizationCommand
    {
        public string Name { get; set; }
        public IList<Address> Addresses { get; set; }
    }
}
