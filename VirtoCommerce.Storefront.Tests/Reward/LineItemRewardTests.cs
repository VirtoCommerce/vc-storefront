using System.Collections.Generic;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Reward
{
    [Trait("Category", "CI")]
    public class LineItemRewardTests
    {
        private const string ExistingProductId = "Pr1";

        [Theory]
        [MemberData(nameof(TestRewards))]
        public void TestApplyRewards(decimal price, int quantity, decimal expectedTotalDiscount, PromotionReward[] rewards)
        {
            var currency = new Currency(Language.InvariantLanguage, "USD");
            var lineItem = new LineItem(currency, Language.InvariantLanguage) { ProductId = ExistingProductId };
            lineItem.ListPrice = new Money(price, currency);
            lineItem.SalePrice = lineItem.ListPrice;
            lineItem.Quantity = quantity;
            lineItem.ApplyRewards(rewards);
            Assert.Equal(expectedTotalDiscount, lineItem.DiscountAmount.Amount);
        }

        public static IEnumerable<object[]> TestRewards
        {
            get
            {
                var samplePromotion = new Promotion() { Id = "PromotionId", Description = "Description" };

                //For 2 out of 3 items with price $100.0 get %50 off
                // No reward - other product
                yield return new object[] { 100.0m, 3, 0m, new[] { new PromotionReward { ProductId = "NonExistentReward", Amount = 50.0m, AmountType = AmountType.Relative, Quantity = 2, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true } } };
                // Full reward - no product specified  =  $33.33 discount on one item
                yield return new object[] { 100.0m, 3, 33.34m, new[] { new PromotionReward { Amount = 50.0m, AmountType = AmountType.Relative, Quantity = 2, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true } } };
                // Full reward - correct product specified  =  $33.33 discount on one item
                yield return new object[] { 100.0m, 3, 33.34m, new[] { new PromotionReward { ProductId = ExistingProductId, Amount = 50.0m, AmountType = AmountType.Relative, Quantity = 2, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // No reward - different reward type
                yield return new object[] { 100.0m, 3, 0m, new[] { new PromotionReward { RewardType = PromotionRewardType.CartSubtotalReward, Amount = 50.0m, AmountType = AmountType.Relative, Quantity = 2, IsValid = true, Promotion = samplePromotion } } };
                // No reward - reward type not set
                yield return new object[] { 100.0m, 3, 0m, new[] { new PromotionReward { Amount = 50.0m, AmountType = AmountType.Relative, Quantity = 2, IsValid = true, Promotion = samplePromotion } } };

                // For 2 in every 3 items of total 10 items with price $100 get %50 off limit 4 items not to exceed $150

                // Quantity variantions
                // For 2 in every 3 items of total 0 items with price $100 get %50 off  =  no discount
                yield return new object[] { 100.0m, 0, 0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 2 in every 3 items of total 1 items with price $100 get %50 off  =  no discount
                yield return new object[] { 100.0m, 1, 0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 2 in every 3 items of total 2 items with price $100 get %50 off  =  no discount
                yield return new object[] { 100.0m, 2, 0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 2 in every 3 items of total 3 items with price $100 get %50 off  =  $33.34 no discount (2 discounted = $100)
                yield return new object[] { 100.0m, 3, 33.34m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 2 in every 3 items of total 4 items with price $100 get %50 off  =  $25 no discount (2 discounted = $100)
                yield return new object[] { 100.0m, 4, 25m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };

                // Money limit
                // Not exceeded, discounted quantity set to 2  =  $10 discount on one item (2 items discounted  = $100 total)
                yield return new object[] { 100.0m, 10, 10.0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, MaxLimit = 150, Quantity = 2, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // $150 limit reached, discounted quantity set to 6  =  $15 discount on one item (4 items discounted, but $150 limit = $150 total discount)
                yield return new object[] { 100.0m, 10, 15.0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, MaxLimit = 150, Quantity = 6, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // Limit not set, discounted quantity set to 10  =  $30 discount on one item (6 of 10 items discounted  =  $300)
                yield return new object[] { 100.0m, 10, 30.0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };

                // Item limit
                // Limit 1 discounted item not to exceed $500 = $30 discount on one item (10 items total with 50 limit = 3 discounted = $300 total discount)
                yield return new object[] { 100.0m, 10, 5.0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, MaxLimit = 500, Quantity = 1, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // Limit 3 discounted items not to exceed $500 = $20 discount on one item (limit 6 items = 4 discounted = $200 total discount)
                yield return new object[] { 100.0m, 10, 15.0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, MaxLimit = 500, Quantity = 3, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // Limit 4 discounted items not to exceed $500 = $20 discount on one item (limit 6 items = 4 discounted = $200 total discount)
                yield return new object[] { 100.0m, 10, 20.0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, MaxLimit = 500, Quantity = 4, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // Limit 6 discounted items not to exceed $500 = $20 discount on one item (limit 6 items = 4 discounted = $200 total discount)
                yield return new object[] { 100.0m, 10, 30.0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, MaxLimit = 500, Quantity = 6, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // Limit 10 discounted items not to exceed $500 = $30 discount on one item (10 items total with 10 limit = 3 discounted = $300 total discount)
                yield return new object[] { 100.0m, 10, 30.0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, MaxLimit = 500, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // No item limit not to exceed $500 = $30 discount on one item (10 items total with no limit = 3 discounted = $300 total discount)
                yield return new object[] { 100.0m, 10, 30.0m, new[] { new PromotionReward { ForNthQuantity = 2, InEveryNthQuantity = 3, MaxLimit = 500, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };

                // For N in every Y items
                // For 1 in every 3 items of total 9 items with price $100 get %50 off limit 10 discounted items not to exceed $500  =  $16.66 per item (3 discounted, 150 / 9)
                yield return new object[] { 100.0m, 9, 16.67m, new[] { new PromotionReward { ForNthQuantity = 1, InEveryNthQuantity = 3, MaxLimit = 500, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 1 in every 3 items of total 10 items with price $100 get %50 off limit 10 discounted items not to exceed $500  =  $15 per item (3 discounted, 150 / 10)
                yield return new object[] { 100.0m, 10, 15.0m, new[] { new PromotionReward { ForNthQuantity = 1, InEveryNthQuantity = 3, MaxLimit = 500, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 1 in every 3 items of total 0 items with price $100 get %50 off limit 10 discounted items not to exceed $500  =  $0 per item ( no items )
                yield return new object[] { 100.0m, 0, 0m, new[] { new PromotionReward { ForNthQuantity = 1, InEveryNthQuantity = 3, MaxLimit = 500, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // If any of ForN or InEveryNth not greater than 0 - calculation should not take both of them into account
                // For 1 in every 1 items of total 10 items with price $100 get %50 off limit 10 discounted items not to exceed $1000  =  $50 per item ( every item discounted by 50 )
                yield return new object[] { 100.0m, 10, 50.0m, new[] { new PromotionReward { ForNthQuantity = 1, InEveryNthQuantity = 1, MaxLimit = 1000, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 0 in every 3 items of total 10 items with price $100 get %50 off limit 10 discounted items not to exceed $1000  =  $0 per item ( ForNthQuantity = 0 )
                yield return new object[] { 100.0m, 10, 50.0m, new[] { new PromotionReward { ForNthQuantity = 0, InEveryNthQuantity = 3, MaxLimit = 1000, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For <null> in every 3 items of total 10 items with price $100 get %50 off limit 10 discounted items not to exceed $1000  =  $0 per item ( ForNthQuantity not set )
                yield return new object[] { 100.0m, 10, 50.0m, new[] { new PromotionReward { InEveryNthQuantity = 3, MaxLimit = 1000, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 1 in every 0 items of total 10 items with price $100 get %50 off limit 10 discounted items not to exceed $1000  =  $0 per item ( InEveryNthQuantity = 0 )
                yield return new object[] { 100.0m, 10, 50.0m, new[] { new PromotionReward { ForNthQuantity = 1, InEveryNthQuantity = 0, MaxLimit = 1000, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 1 in every <null> items of total 10 items with price $100 get %50 off limit 10 discounted items not to exceed $1000  =  $0 per item ( InEveryNthQuantity not set )
                yield return new object[] { 100.0m, 10, 50.0m, new[] { new PromotionReward { ForNthQuantity = 1, MaxLimit = 1000, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 3 in every 2 items of total 10 items with price $100 get %50 off limit 10 discounted items not to exceed $1000  =  $50 per item ( 3 of 2 = all items discounted )
                yield return new object[] { 100.0m, 10, 50.0m, new[] { new PromotionReward { ForNthQuantity = 3, InEveryNthQuantity = 2, MaxLimit = 1000, Quantity = 10, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
                // For 3 in every 2 items of total 10 items with price $100 get %50 off limit 2 discounted items not to exceed $1000  =  $50 per item ( 3 of 2 = all items discounted )
                yield return new object[] { 100.0m, 10, 10.0m, new[] { new PromotionReward { ForNthQuantity = 3, InEveryNthQuantity = 2, MaxLimit = 1000, Quantity = 2, Amount = 50, AmountType = AmountType.Relative, RewardType = PromotionRewardType.CatalogItemAmountReward, IsValid = true, Promotion = samplePromotion } } };
            }
        }
    }
}
