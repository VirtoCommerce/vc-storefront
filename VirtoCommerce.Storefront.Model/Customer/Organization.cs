using System.Runtime.Serialization;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Customer
{
    public partial class Organization : Member
    {
        /// <summary>
        /// Organization contacts
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public IMutablePagedList<Contact> Contacts { get; set; }
    }
}
