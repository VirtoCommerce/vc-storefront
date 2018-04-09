using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using marketingDto = VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{

    public static class MarketingConverterExtension
    {
        public static MarketingConverter MarketingConverterInstance
        {
            get
            {
                return new MarketingConverter();
            }
        }

        public static PromotionEvaluationContext ToPromotionEvaluationContext(this WorkContext workContext, IEnumerable<Product> products = null)
        {
            return MarketingConverterInstance.ToPromotionEvaluationContext(workContext, products);
        }

        public static marketingDto.PromotionEvaluationContext ToPromotionEvaluationContextDto(this PromotionEvaluationContext promoEvaluationContext)
        {
            return MarketingConverterInstance.ToPromotionEvaluationContextDto(promoEvaluationContext);
        }

        public static Promotion ToWebModel(this marketingDto.Promotion promotionDto)
        {
            return MarketingConverterInstance.ToPromotion(promotionDto);
        }       

        public static PromotionReward ToPromotionReward(this marketingDto.PromotionReward rewardDto, Currency currency)
        {
            return MarketingConverterInstance.ToPromotionReward(rewardDto, currency);
        }

        public static DynamicContentItem ToDynamicContentItem(this marketingDto.DynamicContentItem contentItemDto)
        {
            return MarketingConverterInstance.ToDynamicContentItem(contentItemDto);
        }

        public static DynamicProperty ToDynamicProperty(this marketingDto.DynamicObjectProperty propertyDto)
        {
            return MarketingConverterInstance.ToDynamicProperty(propertyDto);
        }

        public static marketingDto.DynamicObjectProperty ToMarketingDynamicPropertyDto(this DynamicProperty property)
        {
            return MarketingConverterInstance.ToMarketingDynamicPropertyDto(property);
        }
    }

    public partial class MarketingConverter
    {
        public virtual DynamicProperty ToDynamicProperty(marketingDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<coreDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public virtual marketingDto.DynamicObjectProperty ToMarketingDynamicPropertyDto(DynamicProperty property)
        {
            return property.ToDynamicPropertyDto().JsonConvert<marketingDto.DynamicObjectProperty>();
        }

        public virtual DynamicContentItem ToDynamicContentItem(marketingDto.DynamicContentItem contentItemDto)
        {
            var result = new DynamicContentItem();

            result.ContentType = contentItemDto.ContentType;
            result.Description = contentItemDto.Description;
            result.FolderId = contentItemDto.FolderId;
            result.Id = contentItemDto.Id;
            result.Name = contentItemDto.Name;
            result.Outline = contentItemDto.Outline;
            result.Path = contentItemDto.Path;

            if (contentItemDto.DynamicProperties != null)
            {
                result.DynamicProperties = contentItemDto.DynamicProperties.Select(ToDynamicProperty).ToList();
            }

            return result;
        }

        public virtual PromotionReward ToPromotionReward(marketingDto.PromotionReward serviceModel, Currency currency)
        {
            var result = new PromotionReward();

            result.CategoryId = serviceModel.CategoryId;
            result.Coupon = serviceModel.Coupon;
            result.Description = serviceModel.Description;
            result.IsValid = serviceModel.IsValid ?? false;
            result.LineItemId = serviceModel.LineItemId;
            result.MeasureUnit = serviceModel.MeasureUnit;
            result.ProductId = serviceModel.ProductId;
            result.PromotionId = serviceModel.PromotionId;
            result.Quantity = serviceModel.Quantity ?? 0;
            
            result.Amount = (decimal)(serviceModel.Amount ?? 0);
            result.AmountType = EnumUtility.SafeParse(serviceModel.AmountType, AmountType.Absolute);
            result.CouponAmount = new Money(serviceModel.CouponAmount ?? 0, currency);
            result.CouponMinOrderAmount = new Money(serviceModel.CouponMinOrderAmount ?? 0, currency);
            result.Promotion = serviceModel.Promotion.ToWebModel();
            result.RewardType = EnumUtility.SafeParse(serviceModel.RewardType, PromotionRewardType.CatalogItemAmountReward);
            result.ShippingMethodCode = serviceModel.ShippingMethod;

            return result;
        }

      

        public virtual Promotion ToPromotion(marketingDto.Promotion promotionDto)
        {
            var result = new Promotion();

            result.Id = promotionDto.Id;      
            result.Name = promotionDto.Name;
            result.Description = promotionDto.Description;
            result.Coupons = promotionDto.Coupons;   

            return result;
        }

        public virtual PromotionEvaluationContext ToPromotionEvaluationContext(WorkContext workContext, IEnumerable<Product> products = null)
        {
            var result = new PromotionEvaluationContext(workContext.CurrentLanguage, workContext.CurrentCurrency)
            {
                Currency = workContext.CurrentCurrency,
                User = workContext.CurrentUser,
                Language = workContext.CurrentLanguage,
                StoreId = workContext.CurrentStore.Id,

                Product = workContext.CurrentProduct
            };

            if (products != null)
            {
                result.Products = products.ToList();
            }

            return result;
        }

        public virtual marketingDto.PromotionEvaluationContext ToPromotionEvaluationContextDto(PromotionEvaluationContext promoEvalContext)
        {
            var result = new marketingDto.PromotionEvaluationContext();

            if (promoEvalContext.Cart != null)
            {
                result.CartPromoEntries = promoEvalContext.Cart.Items.Select(x => x.ToProductPromoEntryDto()).ToList();

                result.CartTotal = (double)promoEvalContext.Cart.SubTotal.Amount;
                result.Coupon = promoEvalContext.Cart.Coupon != null ? promoEvalContext.Cart.Coupon.Code : null;
                result.Currency = promoEvalContext.Cart.Currency.Code;
                result.CustomerId = promoEvalContext.Cart.Customer.Id;                
                result.UserGroups = promoEvalContext.Cart.Customer?.Contact?.UserGroups;
                result.IsRegisteredUser = promoEvalContext.Cart.Customer.IsRegisteredUser;
                result.Language = promoEvalContext.Cart.Language.CultureName;
                //Set cart line items as default promo items
                result.PromoEntries = result.CartPromoEntries;

                if(!promoEvalContext.Cart.Shipments.IsNullOrEmpty())
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
                result.PromoEntries = promoEvalContext.Products.Select(x=>x.ToProductPromoEntryDto()).ToList();
            }

            if(promoEvalContext.Product != null)
            {
                result.PromoEntry = promoEvalContext.Product.ToProductPromoEntryDto();
            }
            
            result.UserGroups = promoEvalContext?.User?.Contact?.UserGroups;

            result.CustomerId = promoEvalContext.User.Id;
            result.IsEveryone = true;
            result.IsRegisteredUser = promoEvalContext.User.IsRegisteredUser;

            result.Currency = promoEvalContext.Currency != null ? promoEvalContext.Currency.Code : null;
            result.Language = promoEvalContext.Language != null ? promoEvalContext.Language.CultureName : null;
            result.StoreId = promoEvalContext.StoreId;
            
            return result;
        }
    }
}
