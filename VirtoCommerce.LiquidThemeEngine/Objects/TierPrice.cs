using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class TierPrice : ValueObject
    {
        public decimal Price { get; set; }

        public long Quantity { get; set; }
    }
}
