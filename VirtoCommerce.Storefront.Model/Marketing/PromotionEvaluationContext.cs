using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Marketing
{
    /// <summary>
    /// Represents context object for promotion evaluation
    /// </summary>
    public partial class PromotionEvaluationContext : ValueObject
    {
        public PromotionEvaluationContext(Language language, Currency currency)
        {
            Language = language;
            Currency = currency;
        }
            
        public string StoreId { get; set; }
        public Language Language { get; set; }
        public Currency Currency { get; set; } 
        public User User { get; set; }
        public Cart.ShoppingCart Cart { get; set; }
        public IList<Product> Products { get; set; } = new List<Product>();
        public Product Product { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StoreId;
            yield return Language;
            yield return Currency;
            yield return User;
            yield return Product;

            if (Cart != null)
            {
                yield return Cart.Total;
                yield return Cart.Coupon;
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
            }
            if (!Products.IsNullOrEmpty())
            {
                foreach (var product in Products)
                {
                    yield return product;                   
                }
            }
        }

    }
}