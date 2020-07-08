using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Customer.Contracts
{
    public class MemberDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MemberType { get; set; }

        public IList<string> Phones { get; set; } = new List<string>();
        public IList<string> Groups { get; set; } = new List<string>();

        public IList<string> Emails { get; set; } = new List<string>();
        public IList<Address> Addresses { get; set; } = new List<Address>();
    }
}
