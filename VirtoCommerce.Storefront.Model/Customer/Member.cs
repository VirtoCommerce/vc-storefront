using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Customer
{
    public class Member : Entity
    {
        public IList<string> PhoneNumbers { get; set; } = new List<string>();
        public string Email
        {
            get
            {
                return Emails.OrderBy(x => x).FirstOrDefault();
            }
        }

        /// <summary>
        /// Returns the email address of the customer.
        /// </summary>
        public IList<string> Emails { get; set; } = new List<string>();

        public string Name { get; set; }
        public string MemberType { get; set; }
        public IList<Address> Addresses { get; set; } = new List<Address>();
        public IList<string> Phones { get; set; } = new List<string>();
        public IList<string> Groups { get; set; } = new List<string>();

        /// <summary>
        /// User groups such as VIP, Wholesaler etc
        /// </summary>
        public IList<string> UserGroups { get; set; } = new List<string>();
        public IList<DynamicProperty> DynamicProperties { get; set; } = new List<DynamicProperty>();

        public override string ToString()
        {
            return $"{MemberType} {Name}";
        }

    }
}
