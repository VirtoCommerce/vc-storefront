using System;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Marketing
{
    public partial class Discount : ValueObject, IConvertible<Discount>
    {
        public Discount()
        {
        }
        public Discount(Currency currency)
        {
            Amount = new Money(currency);
        }

        public string Code => PromotionId;
        /// <summary>
        /// Gets or sets the value of promotion id
        /// </summary>
        public string PromotionId { get; set; }

        /// <summary>
        /// Gets or sets the value of absolute discount amount per one item
        /// </summary>
        public Money Amount { get; set; }

        public decimal Savings => -Amount.Amount;

        public string Coupon { get; set; }

        /// <summary>
        /// Gets or sets the value of discount description
        /// </summary>
        public string Description { get; set; }

        #region IConvertible<Discount> Members
        public Discount ConvertTo(Currency currency)
        {
            var retVal = new Discount(currency)
            {
                PromotionId = PromotionId,
                Description = Description,
                Coupon = Coupon,
                Amount = Amount.ConvertTo(currency)
            };
            return retVal;
        }
        #endregion

        public override object Clone()
        {
            var result = base.Clone() as Discount;
            result.Amount = Amount?.Clone() as Money;
            return result;
        }
    }
}
