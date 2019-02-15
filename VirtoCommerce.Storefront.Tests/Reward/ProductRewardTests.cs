using System.Collections.Generic;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Reward
{
    [Trait("Category", "CI")]
    public class ProductRewardTests
    {
        private const string ExistingProductId = "Pr1";

        [Theory]
        [MemberData(nameof(TestRewards))]
        public void TestRewardAppliance(int expectedDiscountCount, PromotionReward[] rewards)
        {
            var currency = new Currency(Language.InvariantLanguage, "USD");
            var moneyPrice = new Money(1m, currency);
            var productPrice = new ProductPrice(currency)
            {
                ListPrice = moneyPrice,
                SalePrice = moneyPrice,
                TierPrices = new List<TierPrice>()
            };
            var product = new Product(currency, Language.InvariantLanguage)
            {
                Id = ExistingProductId,
                Price = productPrice
            };
            product.ApplyRewards(rewards);

            // Checking only applied discount count here, not disciunt amount
            Assert.Equal(expectedDiscountCount, product.Discounts.Count);
        }

        public static IEnumerable<object[]> TestRewards
        {
            get
            {
                var samplePromotion = new Promotion() { Id = "PromotionId", Description = "Description" };
                const string NonExistingProduct = "Non existing product";

                // RewardType tests
                // 1 CatalogItemAmountReward - expected 1
                yield return new object[] { 1,
                    new[] {
                        new PromotionReward { RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true }
                    }
                };
                // 2 CatalogItemAmountReward - expected 2
                yield return new object[] { 2,
                    new[] {
                        new PromotionReward { RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true }
                    }
                };
                // 2 CatalogItemAmountReward + 1 Cart rewards - expected 2
                yield return new object[] { 2,
                    new[] {
                        new PromotionReward { RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { RewardType = PromotionRewardType.CartSubtotalReward, Promotion = samplePromotion, IsValid = true }
                    }
                };
                // Other types + no type specified  - expected 0
                yield return new object[] { 0,
                    new[] {
                        new PromotionReward { RewardType = PromotionRewardType.GiftReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { RewardType = PromotionRewardType.CartSubtotalReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { RewardType = PromotionRewardType.PaymentReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { RewardType = PromotionRewardType.ShipmentReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { RewardType = PromotionRewardType.SpecialOfferReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { Promotion = samplePromotion, IsValid = true }
                    }
                };

                // Product filtering
                // No product specified - expected 1
                yield return new object[] { 1,
                    new[]
                    {
                        new PromotionReward { RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                    }
                };
                // Existing ProductId specified - expected 1
                yield return new object[] { 1,
                    new[]
                    {
                        new PromotionReward { ProductId = ExistingProductId, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                    }
                };
                // Non Existing ProductId specified - expected 0
                yield return new object[] { 0,
                    new[]
                    {
                        new PromotionReward { ProductId = NonExistingProduct, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                    }
                };

                // Existing ProductId specified and existing ConditionalProductId - expected 1
                yield return new object[] { 1,
                    new[]
                    {
                        new PromotionReward { ProductId = ExistingProductId, ConditionalProductId = ExistingProductId, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                    }
                };
                // Existing ProductId specified and Non Existing ConditionalProductId - expected 1
                yield return new object[] { 1,
                    new[]
                    {
                        new PromotionReward { ProductId = ExistingProductId, ConditionalProductId = NonExistingProduct, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                    }
                };
                // Non Existing ProductId specified and Existing ConditionalProductId - expected 0
                yield return new object[] { 0,
                    new[]
                    {
                        new PromotionReward { ProductId = NonExistingProduct, ConditionalProductId = ExistingProductId, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                    }
                };
                // No ProductId specified and Existing ConditionalProductId - expected 1
                yield return new object[] { 1,
                    new[]
                    {
                        new PromotionReward { ConditionalProductId = ExistingProductId, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                    }
                };
                // Complex scenarion: no product + existing ProductId + existing ProductId and existing ConditionalProductId + non existing ProductId and existing ConditionalProductId - expected 3
                yield return new object[] { 3,
                    new[]
                    {
                        new PromotionReward { RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { ProductId = ExistingProductId, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { ProductId = ExistingProductId, ConditionalProductId = ExistingProductId, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                        new PromotionReward { ProductId = NonExistingProduct, ConditionalProductId = ExistingProductId, RewardType = PromotionRewardType.CatalogItemAmountReward, Promotion = samplePromotion, IsValid = true },
                   }
                };
            }
        }
    }
}
