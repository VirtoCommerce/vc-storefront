using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Models
{
    public class ChangeCartItemPrice
    {
        public string LineItemId { get; set; }
        public decimal NewPrice { get; set; }
    }
}
