using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Reward
{
    [Trait("Category", "CI")]
    public class PromotionRewardTest
    {
        //Jira VP-2194
        [Fact]
        public void TestToDiscountModel()
        {
            //Arrange
            var samplePromotion = new Promotion { Id = "PromotionId", Description = "Description" };
            var promotionReward = new PromotionReward {Amount = 10m, AmountType = AmountType.Relative, Quantity = 1, IsValid = true, Promotion = samplePromotion};
            var currency = new Currency(Language.InvariantLanguage, "USD");

            //Act
            var discount = promotionReward.ToDiscountModel(new Money(4.6m, currency), 1);

            //Assert
            Assert.Equal(0.46m, discount.Amount.Amount);
        }
    }
}
