using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Contracts;

namespace VirtoCommerce.Storefront.Model.Customer.Commands
{
    public class CreateOrganizationCommand
    {
        public string Name { get; set; }
        public IList<AddressDto> Addresses { get; set; }
    }
}
