using System;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class CartShipmentItem : ICloneable
    {
        public LineItem LineItem { get; set; }
        public int Quantity { get; set; }

        #region ICloneable members
        public virtual object Clone()
        {
            var result = MemberwiseClone() as CartShipmentItem;
            result.LineItem = LineItem?.Clone() as LineItem;
            return result;
        }
        #endregion
    }
}
