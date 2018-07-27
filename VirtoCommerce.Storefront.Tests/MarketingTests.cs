using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using Xunit;

namespace VirtoCommerce.Storefront.Tests
{
    public class MarketingTests
    {
        [Fact]
        public void Can_limit_only_relative_rewards()
        {
            var currency = new Currency(new Language("en-US"), "USD (840)");
            var money = new Money(100m, currency);
            //Arrange
            var relativeReward = new PromotionReward()
            {
                Amount = 10,
                AmountType = AmountType.Relative,
                MaxLimit = 8,
                Promotion = new Promotion()
                {
                    Id = "id",
                    Description = ""
                }
            };

            var absoluteReward = new PromotionReward()
            {
                Amount = 10,
                AmountType = AmountType.Absolute,
                MaxLimit = 8,
                Promotion = new Promotion()
                {
                    Id = "id",
                    Description = ""
                }
            };

            //Act

            var relativeDiscount = new Discount();
            var absoluteDiscount = new Discount();


            relativeDiscount = relativeReward.ToDiscountModel(money);
            absoluteDiscount = absoluteReward.ToDiscountModel(money);

            //Assert
            var expectedRelativeDiscountAmount = new Money(8m, currency);
            var expectedAbsoluteDiscountAmount = new Money(10m, currency);
            Assert.Equal(relativeDiscount.Amount, expectedRelativeDiscountAmount);
            Assert.Equal(absoluteDiscount.Amount, expectedAbsoluteDiscountAmount);
        }
    }
}
