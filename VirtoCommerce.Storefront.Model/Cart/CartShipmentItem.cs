using System;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class CartShipmentItem : Entity
    {
        public LineItem LineItem { get; set; }
        public int Quantity { get; set; }
        public override object Clone()
        {
            var result = base.Clone() as CartShipmentItem;

            result.LineItem = LineItem?.Clone() as LineItem;
            return result;
        }

    }
}
