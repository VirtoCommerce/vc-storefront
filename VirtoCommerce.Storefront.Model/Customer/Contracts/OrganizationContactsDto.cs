using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.Customer.Contracts
{
    public class OrganizationContactsDto
    {
        public int TotalCount { get; set; }
        public IList<Contact> Items { get; set; }
    }
}
