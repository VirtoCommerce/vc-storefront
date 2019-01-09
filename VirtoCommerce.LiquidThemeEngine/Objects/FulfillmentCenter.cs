using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class FulfillmentCenter : ValueObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string GeoLocation { get; set; }
        public Address Address { get; set; }
    }
}
