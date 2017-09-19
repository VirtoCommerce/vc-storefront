using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Models
{
    public class ChangeCartItemQty
    {
        public string LineItemId { get; set; }
        public int Quantity { get; set; }
    }
}
