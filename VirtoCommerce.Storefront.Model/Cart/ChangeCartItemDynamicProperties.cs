using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class ChangeCartItemDynamicProperties
    {
        public string LineItemId { get; set; }
        public Dictionary<string, string> DynamicProperties { get; set; }
    }
}
