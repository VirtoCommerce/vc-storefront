using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Contracts;

namespace VirtoCommerce.Storefront.Model.Customer.Contracts
{
    public class MemberDto
    {
        public IList<string> PhoneNumbers { get; set; } = new List<string>();
        public IList<string> Emails { get; set; } = new List<string>();

        public string Name { get; set; }
        public IList<AddressDto> Addresses { get; set; } = new List<AddressDto>();
    }
}
