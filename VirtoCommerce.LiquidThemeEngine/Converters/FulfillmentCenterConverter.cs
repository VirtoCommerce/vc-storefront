using VirtoCommerce.LiquidThemeEngine.Objects;
using storefrontModel = VirtoCommerce.Storefront.Model.Inventory;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{

    public static partial class FulfillmentCenterConverter
    {
        public static FulfillmentCenter ToShopifyModel(this storefrontModel.FulfillmentCenter center)
        {
            var result = new FulfillmentCenter()
            {
                Description = center.Description,
                GeoLocation = center.GeoLocation,
                Name = center.Name
            };

            if(center.Address != null)
            {
                result.Address = center.Address.ToShopifyModel();
            }

            return result;
        }
    }

}
