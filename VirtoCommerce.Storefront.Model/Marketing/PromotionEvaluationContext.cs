using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Marketing
{
    /// <summary>
    /// Represents context object for promotion evaluation
    /// </summary>
    public partial class PromotionEvaluationContext : MarketingEvaluationContextBase
    {
        public PromotionEvaluationContext(Language language, Currency currency)
            : base(language, currency)
        {
        }

        public Cart.ShoppingCart Cart { get; set; }
        public IList<Product> Products { get; set; } = new List<Product>();
        public Product Product { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var baseItem in base.GetEqualityComponents())
            {
                yield return baseItem;
            }

            if (Cart != null)
            {
                yield return Cart.Total;
                yield return Cart.Currency;
                yield return Cart.Language;

                if (!Cart.Items.IsNullOrEmpty())
                {
                    foreach (var lineItem in Cart.Items)
                    {
                        yield return lineItem;
                        yield return lineItem.ProductId;
                        yield return lineItem.Quantity;
                        yield return lineItem.PlacedPrice;
                        yield return lineItem.InStockQuantity;
                    }
                }
                if (!Cart.Shipments.IsNullOrEmpty())
                {
                    foreach (var shipment in Cart.Shipments)
                    {
                        yield return shipment.ShipmentMethodCode;
                        yield return shipment.ShipmentMethodOption;
                        yield return shipment.Price;
                    }
                }
                if (!Cart.Payments.IsNullOrEmpty())
                {
                    foreach (var shipment in Cart.Payments)
                    {
                        yield return shipment.PaymentGatewayCode;
                        yield return shipment.Price;

                    }
                }
                if (!Cart.Coupons.IsNullOrEmpty())
                {
                    foreach (var coupon in Cart.Coupons)
                    {
                        yield return coupon;

                    }
                }
            }
            if (!Products.IsNullOrEmpty())
            {
                foreach (var product in Products)
                {
                    yield return product;
                }
            }
            yield return Product;
        }

    }
}
