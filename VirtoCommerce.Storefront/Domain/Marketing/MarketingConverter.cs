using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using marketingDto = VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi.Models;
using platformDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static partial class MarketingConverter
    {
        public static DynamicProperty ToDynamicProperty(this marketingDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<platformDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public static marketingDto.DynamicObjectProperty ToMarketingDynamicPropertyDto(this DynamicProperty property)
        {
            return property.ToDynamicPropertyDto().JsonConvert<marketingDto.DynamicObjectProperty>();
        }

        public static DynamicContentItem ToDynamicContentItem(this marketingDto.DynamicContentItem contentItemDto)
        {
            var result = new DynamicContentItem
            {
                ContentType = contentItemDto.ContentType,
                Description = contentItemDto.Description,
                FolderId = contentItemDto.FolderId,
                Id = contentItemDto.Id,
                Name = contentItemDto.Name,
                Outline = contentItemDto.Outline,
                Path = contentItemDto.Path
            };

            if (contentItemDto.DynamicProperties != null)
            {
                result.DynamicProperties = contentItemDto.DynamicProperties.Select(ToDynamicProperty).ToList();
            }

            return result;
        }

        public static PromotionReward ToPromotionReward(this marketingDto.PromotionReward rewardDto, Currency currency)
        {
            var result = new PromotionReward
            {
                CategoryId = rewardDto.CategoryId,
                Coupon = rewardDto.Coupon,
                Description = rewardDto.Description,
                IsValid = rewardDto.IsValid ?? false,
                LineItemId = rewardDto.LineItemId,
                MeasureUnit = rewardDto.MeasureUnit,
                ProductId = rewardDto.ProductId,
                PromotionId = rewardDto.PromotionId,
                Quantity = rewardDto.Quantity ?? 0,
                MaxLimit = (decimal)(rewardDto.MaxLimit ?? 0),
                Amount = (decimal)(rewardDto.Amount ?? 0),
                AmountType = EnumUtility.SafeParse(rewardDto.AmountType, AmountType.Absolute),
                CouponAmount = new Money(rewardDto.CouponAmount ?? 0, currency),
                CouponMinOrderAmount = new Money(rewardDto.CouponMinOrderAmount ?? 0, currency),
                Promotion = rewardDto.Promotion.ToPromotion(),
                RewardType = EnumUtility.SafeParse(rewardDto.RewardType, PromotionRewardType.CatalogItemAmountReward),
                ShippingMethodCode = rewardDto.ShippingMethod,
                ConditionalProductId = rewardDto.ConditionalProductId,
                ForNthQuantity = rewardDto.ForNthQuantity,
                InEveryNthQuantity = rewardDto.InEveryNthQuantity,
            };

            return result;
        }

        public static Promotion ToPromotion(this marketingDto.Promotion promotionDto)
        {
            var result = new Promotion
            {
                Id = promotionDto.Id,
                Name = promotionDto.Name,
                Description = promotionDto.Description,
            };

            return result;
        }

        public static DynamicContentEvaluationContext ToDynamicContentEvaluationContext(this WorkContext workContext, IEnumerable<Product> products = null)
        {
            var result = new DynamicContentEvaluationContext(workContext.CurrentLanguage, workContext.CurrentCurrency)
            {
                User = workContext.CurrentUser,
                StoreId = workContext.CurrentStore.Id,
            };
            return result;
        }

        public static PromotionEvaluationContext ToPromotionEvaluationContext(this WorkContext workContext, IEnumerable<Product> products = null)
        {
            var result = new PromotionEvaluationContext(workContext.CurrentLanguage, workContext.CurrentCurrency)
            {
                User = workContext.CurrentUser,
                StoreId = workContext.CurrentStore.Id,
                Product = workContext.CurrentProduct
            };
            if (products != null)
            {
                result.Products = products.ToList();
            }
            return result;
        }

        public static marketingDto.DynamicContentEvaluationContext ToDynamicContentEvaluationContextDto(this DynamicContentEvaluationContext dynamicContentEvalContext)
        {
            var result = new marketingDto.DynamicContentEvaluationContext
            {
                UserGroups = dynamicContentEvalContext?.User?.Contact?.UserGroups,
                Language = dynamicContentEvalContext?.Language != null ? dynamicContentEvalContext.Language.CultureName : null,
                StoreId = dynamicContentEvalContext?.StoreId,
                Tags = dynamicContentEvalContext?.Tags,
                ToDate = dynamicContentEvalContext?.ToDate,
                PlaceName = dynamicContentEvalContext?.PlaceName
            };
            return result;
        }


        public static marketingDto.PromotionEvaluationContext ToPromotionEvaluationContextDto(this PromotionEvaluationContext promoEvalContext)
        {
            var result = new marketingDto.PromotionEvaluationContext();

            if (promoEvalContext.Cart != null)
            {
                result.CartPromoEntries = promoEvalContext.Cart.Items.Select(x => x.ToProductPromoEntryDto()).ToList();

                result.CartTotal = (double)promoEvalContext.Cart.SubTotal.Amount;
                result.Coupons = promoEvalContext.Cart.Coupons?.Select(c => c.Code).ToList();
                result.Currency = promoEvalContext.Cart.Currency.Code;
                result.CustomerId = promoEvalContext.Cart.CustomerId;
                result.UserGroups = promoEvalContext.Cart.Customer?.Contact?.UserGroups;
                result.IsRegisteredUser = promoEvalContext.Cart.Customer?.IsRegisteredUser;
                result.Language = promoEvalContext.Cart.Language.CultureName;
                //Set cart line items as default promo items
                result.PromoEntries = result.CartPromoEntries;

                if (!promoEvalContext.Cart.Shipments.IsNullOrEmpty())
                {
                    var shipment = promoEvalContext.Cart.Shipments.First();
                    result.ShipmentMethodCode = shipment.ShipmentMethodCode;
                    result.ShipmentMethodOption = shipment.ShipmentMethodOption;
                    result.ShipmentMethodPrice = (double)shipment.Price.Amount;
                }
                if (!promoEvalContext.Cart.Payments.IsNullOrEmpty())
                {
                    var payment = promoEvalContext.Cart.Payments.First();
                    result.PaymentMethodCode = payment.PaymentGatewayCode;
                    result.PaymentMethodPrice = (double)payment.Price.Amount;
                }
            }

            if (!promoEvalContext.Products.IsNullOrEmpty())
            {
                result.PromoEntries = promoEvalContext.Products.Select(x => x.ToProductPromoEntryDto()).ToList();
            }

            if (promoEvalContext.Product != null)
            {
                result.PromoEntry = promoEvalContext.Product.ToProductPromoEntryDto();
            }

            result.UserGroups = promoEvalContext?.User?.Contact?.UserGroups;

            result.CustomerId = promoEvalContext.User.Id;
            result.IsEveryone = true;
            result.IsRegisteredUser = promoEvalContext.User.IsRegisteredUser;
            result.IsFirstTimeBuyer = promoEvalContext.User.IsFirstTimeBuyer;

            result.Currency = promoEvalContext.Currency != null ? promoEvalContext.Currency.Code : null;
            result.Language = promoEvalContext.Language != null ? promoEvalContext.Language.CultureName : null;
            result.StoreId = promoEvalContext.StoreId;

            return result;
        }
    }
}
