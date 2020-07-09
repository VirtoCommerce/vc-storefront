using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.Customer.Contracts
{
    public class OrganizationDto : Organization
    {
       public new  OrganizationContactsDto Contacts { get; set; }
    }
}
