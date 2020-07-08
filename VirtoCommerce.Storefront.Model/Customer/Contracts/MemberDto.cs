using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Customer.Contracts
{
    public class MemberDto
    {
        public IList<string> PhoneNumbers { get; set; } = new List<string>();
        public IList<string> Emails { get; set; } = new List<string>();

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        public IList<Address> Addresses { get; set; } = new List<Address>();
    }
}
