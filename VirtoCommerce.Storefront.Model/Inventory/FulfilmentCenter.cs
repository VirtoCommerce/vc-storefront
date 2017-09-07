using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Inventory
{
    public partial class FulfillmentCenter : Entity
    {
        public string Name { get; set; }
        public Address Address { get; set; }
    }
}
