using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Subscriptions;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class ShoppingCartDto
    {
        public string Id { get; set; }

        public string ValidationRuleSet { get; set; } = "default, strict";

        public string Name { get; set; }

        public string Status { get; set; }

        public string StoreId { get; set; }

        public string ChannelId { get; set; }

        public bool HasPhysicalProducts { get; set; }

        public bool IsAnonymous { get; set; }

        public User Customer { get; set; }

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string OrganizationId { get; set; }

        public bool? IsRecuring { get; set; }

        public string Comment { get; set; }

        [JsonIgnore]
        public string Note => Comment;

        public decimal? VolumetricWeight { get; set; }

        public string WeightUnit { get; set; }

        public decimal? Weight { get; set; }

        //public string MeasureUnit { get; set; }

        //public decimal Height { get; set; }

        //public decimal Length { get; set; }

        //public decimal Width { get; set; }

        public Money Total { get; set; }

        public Money SubTotal { get; set; }

        public Money SubTotalWithTax { get; set; }

        public Money ShippingPrice { get; set; }

        public Money ShippingPriceWithTax { get; set; }

        public Money ShippingTotal { get; set; }

        public Money ShippingTotalWithTax { get; set; }

        public Money PaymentPrice { get; set; }

        public Money PaymentPriceWithTax { get; set; }

        public Money PaymentTotal { get; set; }

        public Money PaymentTotalWithTax { get; set; }

        //public Money ExtendedPriceTotal { get; set; }

        //public Money ExtendedPriceTotalWithTax { get; set; }

        public Money HandlingTotal { get; set; }
        public Money HandlingTotalWithTax { get; set; }

        public Money DiscountTotal { get; set; }

        public Money DiscountTotalWithTax { get; set; }

        public IList<Address> Addresses { get; set; }

        public IList<LineItem> Items { get; set; }

        public int ItemsCount { get; set; }

        public int ItemsQuantity { get; set; }

        public Coupon Coupon { get; set; }

        public IList<Coupon> Coupons { get; set; }

        public IList<Payment> Payments { get; set; }

        public IList<Shipment> Shipments { get; set; }

        public string ObjectType { get; set; }

        public IList<DynamicProperty> DynamicProperties { get; set; }

        public IList<PaymentMethod> AvailablePaymentMethods { get; set; }

        public LineItem RecentlyAddedItem { get; set; }

        public PaymentPlan PaymentPlan { get; set; }

        #region IValidatable Members

        public bool IsValid { get; set; } = true;
        public IList<ValidationError> ValidationErrors { get; set; }

        #endregion IValidatable Members

        #region IDiscountable Members

        public IList<DiscountDto> Discounts { get; set; }

        public Currency Currency { get; set; }

        #endregion IDiscountable Members

        #region ITaxable Members

        public Money TaxTotal { get; set; }

        public decimal TaxPercentRate { get; set; }

        public string TaxType { get; set; }

        public IList<TaxDetail> TaxDetails { get; set; }

        public string Type { get; set; }

        #endregion ITaxable Members

        #region IHasLanguage Members

        public Language Language { get; set; }

        #endregion IHasLanguage Members

        public IList<ShippingMethod> AvailableShippingMethods { get; set; }
    }
}
