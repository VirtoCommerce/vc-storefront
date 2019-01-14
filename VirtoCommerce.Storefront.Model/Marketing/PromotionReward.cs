using System;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Marketing
{
    /// <summary>
    /// Represents promotion reward object
    /// </summary>
    public partial class PromotionReward
    {
        /// <summary>
        /// Gets or sets promotion reward amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets promotion reward amount type (absolute or relative)
        /// </summary>
        public AmountType AmountType { get; set; }

        /// <summary>
        /// Gets or sets category ID promotion reward
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// Gets or sets promotion reward coupon code
        /// </summary>
        public string Coupon { get; set; }

        /// <summary>
        /// Gets or sets promotion reward coupon amount
        /// </summary>
        public Money CouponAmount { get; set; }

        /// <summary>
        /// Gets or sets promotion reward minimum order amount for applying coupon
        /// </summary>
        public Money CouponMinOrderAmount { get; set; }

        /// <summary>
        /// Gets or sets the description of promotion reward
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the image URL of promotion reward
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the sign that promotion reward is valid (for dynamic discount) or not (for potential discount)
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets line item ID for which promotion reward was applied
        /// </summary>
        public string LineItemId { get; set; }

        /// <summary>
        /// Gets or sets the measurement unit for promotion reward
        /// </summary>
        public string MeasureUnit { get; set; }

        /// <summary>
        /// Gets or sets product ID for which promotion reward was applied
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the promotion for reward
        /// </summary>
        public Promotion Promotion { get; set; }

        /// <summary>
        /// Gets or sets promotion ID for reward
        /// </summary>
        public string PromotionId { get; set; }

        /// <summary>
        /// Gets or set the quantity of items
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets promotion reward type
        /// </summary>
        public PromotionRewardType RewardType { get; set; }

        /// <summary>
        /// Gets or sets the shipping method code for the marketing reward
        /// </summary>
        public string ShippingMethodCode { get; set; }

        /// <summary>
        /// Gets or sets the payment method code for the marketing reward
        /// </summary>
        public string PaymentMethodCode { get; set; }

        /// <summary>
        /// Gets or sets the max limit for relative rewards
        /// </summary>
        public decimal MaxLimit { get; set; }

        /// <summary>
        /// Gets or sets ConditionalProductId in
        /// For N items of entry ProductId  in every Y items of entry
        /// ConditionalProductId get %X off
        /// </summary>
        public string ConditionalProductId { get; set; }

        /// <summary>
        /// Gets or sets N quantity in
        /// For N items of entry ProductId  in every Y items of entry
        /// ConditionalProductId get %X off
        /// </summary>
        public int? ForNthQuantity { get; set; }

        /// <summary>
        /// Gets or sets Y quantity in
        /// For N items of entry ProductId  in every Y items of entry
        /// ConditionalProductId get %X off
        /// </summary>
        public int? InEveryNthQuantity { get; set; }

        /// <summary>
        /// Get per item reward discount for given item price and quantity.
        /// </summary>
        /// <param name="price">Price per item.</param>
        /// <param name="quantity">Total item quantity.</param>
        /// <returns>Absolute reward discount per item.</returns>
        public Discount ToDiscountModel(Money price, int quantity = 1)
        {
            var discountPerItem = GetDiscountPerItem(price, quantity);
            var discount = new Discount(price.Currency)
            {
                Amount = discountPerItem,
                Description = Promotion.Description,
                Coupon = Coupon,
                PromotionId = Promotion.Id
            };

            return discount;
        }

        // Similar to vc-module-core/AmountBasedReward.GetRewardAmount 
        private Money GetDiscountPerItem(Money price, int quantity)
        {
            var decimalPrice = price.Amount;
            if (decimalPrice < 0)
            {
                throw new ArgumentNullException($"The {nameof(decimalPrice)} cannot be negative");
            }
            if (quantity < 0)
            {
                throw new ArgumentNullException($"The {nameof(quantity)} cannot be negative");
            }

            var workQuantity = quantity = Math.Max(1, quantity);
            if (ForNthQuantity > 0 && InEveryNthQuantity > 0)
            {
                workQuantity = workQuantity / (InEveryNthQuantity ?? 1) * (ForNthQuantity ?? 1);
            }
            if (Quantity > 0)
            {
                workQuantity = Math.Min(Quantity, workQuantity);
            }
            var result = Amount * workQuantity;
            if (AmountType == AmountType.Relative)
            {
                result = decimalPrice * Amount * 0.01m * workQuantity;
            }
            var totalCost = decimalPrice * quantity;
            //use total cost as MaxLimit if it explicitly not set
            var workMaxLimit = MaxLimit > 0 ? MaxLimit : totalCost;
            //Do not allow maxLimit be greater that total cost (to prevent reward amount be greater that price)
            workMaxLimit = Math.Min(workMaxLimit, totalCost);
            result = Math.Min(workMaxLimit, result);

            return new Money(result, price.Currency).Allocate(quantity).FirstOrDefault();
        }
    }
}
