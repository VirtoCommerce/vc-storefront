using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Order;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class ShippingMethodConverter
    {
        public static ShippingMethod ToShopifyModel(this Shipment shipment)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidShippingMethod(shipment);
        }

        public static ShippingMethod ToShopifyModel(this Storefront.Model.ShippingMethod shippingMethod)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidShippingMethod(shippingMethod);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual ShippingMethod ToLiquidShippingMethod(Shipment shipment)
        {
            var result = new ShippingMethod();
            result.Price = shipment.Total.Amount * 100;
            result.PriceWithTax = shipment.TotalWithTax.Amount * 100;
            result.Title = shipment.ShipmentMethodCode;
            result.Handle = shipment.ShipmentMethodCode;

            return result;
        }

        public virtual ShippingMethod ToLiquidShippingMethod(Storefront.Model.ShippingMethod shippingMethod)
        {
            var result = new ShippingMethod();

            result.Handle = shippingMethod.ShipmentMethodCode;
            result.Price = shippingMethod.Price.Amount;
            result.TaxType = shippingMethod.TaxType;
            result.Title = shippingMethod.Name;

            return result;
        }
    }
}