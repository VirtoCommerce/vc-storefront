using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using Xunit;

namespace VirtoCommerce.Storefront.Tests
{
    [Trait("Category", "Unit")]
    public class MarketingTests
    {
        [Theory]
        [InlineData(10, 8, 8, AmountType.Relative)]
        [InlineData(10, 8, 10, AmountType.Absolute)]
        public void Can_limit_only_relative_rewards(int amount, int maxLimit, decimal expected, AmountType type)
        {
            var currency = new Currency(new Language("en-US"), "USD (840)");
            var money = new Money(100m, currency);
            //Arrange
            var reward = new PromotionReward()
            {
                Amount = amount,
                AmountType = type,
                MaxLimit = maxLimit,
                Promotion = new Promotion()
                {
                    Id = "id",
                    Description = ""
                }
            };

            //Act
            var discount = new Discount();
            discount = reward.ToDiscountModel(money);

            //Assert
            var expectedAmount = new Money(expected, currency);
            Assert.Equal(discount.Amount, expectedAmount);
        }
    }
}
