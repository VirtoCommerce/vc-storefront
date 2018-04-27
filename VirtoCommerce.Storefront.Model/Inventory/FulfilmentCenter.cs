using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Inventory
{
    public partial class FulfillmentCenter : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string GeoLocation { get; set; }
        public Address Address { get; set; }
    }
}
