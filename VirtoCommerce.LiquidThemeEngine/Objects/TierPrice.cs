using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class TierPrice : Drop
    {
        public decimal Price { get; set; }

        public long Quantity { get; set; }
    }
}