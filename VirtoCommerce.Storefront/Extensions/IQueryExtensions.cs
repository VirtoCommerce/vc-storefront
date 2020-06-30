using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using GraphQL.Query.Builder;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Subscriptions;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class IQueryExtensions
    {
        public static IQuery<T> AddContext<T, S>(this IQuery<T> query, S context) where T : class
        {
            var attributes = typeof(S).GetCustomAttributes(false);
            var dataContractAttribute = (DataContractAttribute)attributes.FirstOrDefault(a => a is DataContractAttribute);
            if (dataContractAttribute == null)
            {
                throw new ArgumentException($"{typeof(S).Name} should have DataContractAttribute");
            }

            var dataMemberProperties = typeof(S).GetProperties().Where(p => p.GetCustomAttributes().Any(a => a is DataMemberAttribute));
            if (!dataMemberProperties.Any())
            {
                throw new ArgumentException($"{typeof(S).Name} should have at least one property marked with DataMemberAttribute");
            }

            var result = new StringBuilder();

            foreach (var property in dataMemberProperties)
            {
                result.Append($"{((DataMemberAttribute)property.GetCustomAttribute(typeof(DataMemberAttribute))).Name}:\"{property.GetValue(context)}\",");
            }
            result.Remove(result.Length - 1, 1);
            query.AddArgument(dataContractAttribute.Name, result.ToString());

            return query;
        }

        public static IQuery<T> AddCartArguments<T>(this IQuery<T> query, Model.Cart.CartSearchCriteria criteria) where T : class
        {
            query.AddArguments(new
            {
                storeId = criteria.StoreId,
                cartName = criteria.Name,
                userId = criteria.Customer?.Id,
                cultureName = criteria.Language?.CultureName ?? "en-US",
                currencyCode = criteria.Currency.Code,
                type = criteria.Type ?? string.Empty
            });

            return query;
        }

        public static IQuery<T> AddMoneyField<T>(this IQuery<T> query, Expression<Func<T, Money>> selector) where T : class
        {
            query.AddField(
                selector,
                t => t
                       .AddField(v => v.Amount)
                       .AddField(v => v.DecimalDigits)
                       .AddField(v => v.FormattedAmount)
                       .AddField(v => v.FormattedAmountWithoutPoint)
                       .AddField(v => v.FormattedAmountWithoutCurrency)
                       .AddField(v => v.FormattedAmountWithoutPointAndCurrency));

            return query;
        }

        public static IQuery<T> AddAddressField<T>(this IQuery<T> query, Expression<Func<T, Model.Address>> selector) where T : class
        {
            query.AddField(
                selector,
                t => t.FillAddressField());

            return query;
        }

        private static IQuery<Model.Address> FillAddressField(this IQuery<Model.Address> query)
        {
            return query.AddField(x => x.Key)
                    .AddField(x => x.Name)
                    .AddField(x => x.Organization)
                    .AddField(x => x.CountryCode)
                    .AddField(x => x.CountryName)
                    .AddField(x => x.City)
                    .AddField(x => x.PostalCode)
                    .AddField(x => x.Zip)
                    .AddField(x => x.Line1)
                    .AddField(x => x.Line2)
                    .AddField(x => x.RegionId)
                    .AddField(x => x.RegionName)
                    .AddField(x => x.FirstName)
                    .AddField(x => x.MiddleName)
                    .AddField(x => x.LastName)
                    .AddField(x => x.Phone)
                    .AddField(x => x.Email);
        }

        public static IQuery<T> AddCurrencyField<T>(this IQuery<T> query, Expression<Func<T, Currency>> selector) where T : class
        {
            query.AddField(
                    selector,
                    t => t
                        .AddField(v => v.Code)
                        .AddField(v => v.CultureName)
                        .AddField(v => v.Symbol)
                        .AddField(v => v.EnglishName)
                        .AddField(v => v.ExchangeRate)
                        //.AddField(v => v.CustomFormatting) TODO: fix "GraphQL.ExecutionError: Cannot return null for non-null type. Field: customFormatting, Type: String!
                        );

            return query;
        }

        public static IQuery<T> AddPaymentPlanField<T>(this IQuery<T> query, Expression<Func<T, PaymentPlan>> selector) where T : class
        {
            query.AddField(
                    selector,
                    t => t
                        .AddField(v => v.Interval)
                        .AddField(v => v.IntervalCount)
                        .AddField(v => v.TrialPeriodDays));

            return query;
        }

        public static IQuery<T> AddCouponField<T>(this IQuery<T> query, Expression<Func<T, Coupon>> selector) where T : class
        {
            query.AddField(
                    selector,
                    t => t.FillCouponField());

            return query;
        }

        private static IQuery<Coupon> FillCouponField(this IQuery<Coupon> query)
        {
            return query.AddField(x => x.Code)
                        .AddField(x => x.Description)
                        .AddField(x => x.AppliedSuccessfully)
                        .AddField(x => x.ErrorCode);
        }

        public static IQuery<T> AddLineItemField<T>(this IQuery<T> query, Expression<Func<T, LineItem>> selector) where T : class
        {
            query.AddField(
                    selector,
                    t => t.FillLineItemField());

            return query;
        }

        private static IQuery<LineItem> FillLineItemField(this IQuery<LineItem> query)
        {
            return query.AddField(x => x.CreatedDate)
                       //.AddField(x => x.Product) //TODO: need implementation
                       .AddField(x => x.ProductId)
                       .AddField(x => x.ProductType)
                       .AddField(x => x.CatalogId)
                       .AddField(x => x.CategoryId)
                       .AddField(x => x.Sku)
                       .AddField(x => x.Name)
                       .AddField(x => x.Quantity)
                       .AddField(x => x.InStockQuantity)
                       .AddField(x => x.WarehouseLocation)
                       .AddField(x => x.ShipmentMethodCode)
                       .AddField(x => x.RequiredShipping)
                       .AddField(x => x.ThumbnailImageUrl)
                       .AddField(x => x.ImageUrl)
                       .AddField(x => x.IsGift)
                       .AddField(x => x.LanguageCode)
                       .AddField(x => x.Comment)
                       .AddField(x => x.IsReccuring)
                       .AddField(x => x.VolumetricWeight)
                       .AddField(x => x.WeightUnit)
                       .AddField(x => x.Weight)
                       .AddField(x => x.MeasureUnit)
                       .AddField(x => x.Height)
                       .AddField(x => x.Length)
                       .AddField(x => x.Width)
                       .AddField(x => x.IsReadOnly)
                       .AddMoneyField(x => x.ListPrice)
                       //.AddPaymentPlanField(x => x.PaymentPlan) //TODO: fix bug PaymentPlanType, r. 39
                       .AddMoneyField(x => x.ListPriceWithTax)
                       .AddMoneyField(x => x.SalePrice)
                       .AddMoneyField(x => x.SalePriceWithTax)
                       .AddMoneyField(x => x.PlacedPrice)
                       .AddMoneyField(x => x.PlacedPriceWithTax)
                       .AddMoneyField(x => x.ExtendedPrice)
                       .AddMoneyField(x => x.ExtendedPriceWithTax)
                       .AddMoneyField(x => x.DiscountAmount)
                       .AddMoneyField(x => x.DiscountAmountWithTax)
                       .AddMoneyField(x => x.DiscountTotal)
                       .AddMoneyField(x => x.DiscountTotalWithTax)
                       .AddField(x => x.ObjectType)
                       .AddField(x => x.IsValid)
                       .AddValidationErrorsField(x => x.ValidationErrors)
                       .AddMoneyField(x => x.TaxTotal)
                       .AddField(x => x.TaxPercentRate)
                       .AddField(x => x.TaxType)
                       .AddTaxDetailsField(x => x.TaxDetails);
        }

        public static IQuery<T> AddTaxDetailsField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<Model.TaxDetail>>> selector) where T : class
        {
            query.AddField<Model.TaxDetail>(
                    selector,
                    t => t
                        .AddMoneyField(v => v.Rate)
                        .AddMoneyField(v => v.Amount)
                        //.AddField(v => v.Name) //TODO: fix "GraphQL.Validation.ValidationError: Field name of type MoneyType must have a sub selection"
                        //.AddField(v => v.Price)) //TODO: fix "GraphQL.Validation.ValidationError: Field price of type MoneyType must have a sub selection"
                        );

            return query;
        }

        public static IQuery<T> AddShipmentsField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<Shipment>>> selector) where T : class
        {
            query.AddField<Shipment>(
                    selector,
                    t => t
                       .AddField(v => v.ShipmentMethodCode)
                       .AddField(v => v.ShipmentMethodOption)
                       .AddField(v => v.FulfillmentCenterId)
                       .AddAddressField(v => v.DeliveryAddress)
                       .AddField(x => x.VolumetricWeight)
                       .AddField(x => x.WeightUnit)
                       .AddField(x => x.Weight)
                       .AddField(x => x.MeasureUnit)
                       .AddField(x => x.Height)
                       .AddField(x => x.Length)
                       .AddField(x => x.Width)
                       .AddMoneyField(x => x.Price)
                       .AddMoneyField(x => x.PriceWithTax)
                       .AddMoneyField(x => x.Total)
                       .AddMoneyField(x => x.TotalWithTax)
                       .AddMoneyField(x => x.DiscountAmount)
                       .AddMoneyField(x => x.DiscountAmountWithTax));

            return query;
        }

        public static IQuery<T> AddAvailableShippingMethodsField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<Model.ShippingMethod>>> selector) where T : class
        {
            query.AddField<Model.ShippingMethod>(
                    selector,
                    t => t
                       .AddField(x => x.ShipmentMethodCode)
                       .AddField(x => x.Name)
                       .AddField(x => x.LogoUrl)
                       .AddField(x => x.OptionName)
                       .AddField(x => x.OptionDescription)
                       .AddField(x => x.Priority)
                       .AddSettingsField(x => x.Settings)
                       .AddCurrencyField(x => x.Currency)
                       .AddMoneyField(x => x.Price)
                       .AddMoneyField(x => x.DiscountAmount)
                       .AddField(x => x.TaxPercentRate)
                       .AddField(x => x.TaxType)
                       .AddTaxDetailsField(x => x.TaxDetails)
                       .AddDiscountsField(x => x.Discounts));

            return query;
        }

        public static IQuery<T> AddSettingsField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<Model.SettingEntry>>> selector) where T : class
        {
            query.AddField<Model.SettingEntry>(
                    selector,
                    t => t
                         .AddField(x => x.Name)
                         //.AddField(x => x.Value) //TODO: fix GraphQL.Validation.ValidationError: Field value of type Query must have a sub selection
                         .AddField(x => x.ValueType)
                        );

            return query;
        }

        public static IQuery<T> AddItemsField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<LineItem>>> selector) where T : class
        {
            query.AddField<LineItem>(
                    selector,
                    t => t.FillLineItemField());

            return query;
        }

        public static IQuery<T> AddValidationErrorsField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<ValidationError>>> selector) where T : class
        {
            query.AddField<ValidationError>(
                    selector,
                    t => t
                         .AddField(x => x.ErrorCode));

            return query;
        }

        public static IQuery<T> AddDiscountsField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<Model.Marketing.Discount>>> selector) where T : class
        {
            query.AddField<Model.Marketing.Discount>(
                    selector,
                    t => t
                         .AddField(x => x.PromotionId)
                         .AddMoneyField(x => x.Amount)
                         .AddField(x => x.Coupon)
                         .AddField(x => x.Description));

            return query;
        }

        public static IQuery<T> AddPaymentsField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<Payment>>> selector) where T : class
        {
            query.AddField<Payment>(
                    selector,
                    t => t
                         .AddField(x => x.OuterId)
                         .AddField(x => x.PaymentGatewayCode)
                         .AddCurrencyField(x => x.Currency)
                         .AddMoneyField(x => x.Amount)
                         .AddAddressField(x => x.BillingAddress)
                         .AddMoneyField(x => x.Price)
                         .AddMoneyField(x => x.PriceWithTax)
                         .AddMoneyField(x => x.Total)
                         .AddMoneyField(x => x.TotalWithTax)
                         .AddMoneyField(x => x.DiscountAmount)
                         .AddMoneyField(x => x.DiscountAmountWithTax)
                         .AddMoneyField(x => x.TaxTotal)
                         .AddField(x => x.TaxPercentRate)
                         .AddField(x => x.TaxType)
                         .AddTaxDetailsField(x => x.TaxDetails)
                         .AddDiscountsField(x => x.Discounts));

            return query;
        }

        public static IQuery<T> AddAvailablePaymentMethodsField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<Model.PaymentMethod>>> selector) where T : class
        {
            query.AddField<Model.PaymentMethod>(
                    selector,
                    t => t
                         .AddField(x => x.Code)
                         .AddField(x => x.Name)
                         .AddField(x => x.LogoUrl)
                         .AddField(x => x.Description)
                         .AddField(x => x.PaymentMethodType)
                         .AddField(x => x.PaymentMethodGroupType)
                         .AddField(x => x.Priority)
                         .AddField(x => x.IsAvailableForPartial)
                         .AddSettingsField(x => x.Settings)
                         .AddCurrencyField(x => x.Currency)
                         .AddMoneyField(x => x.Price)
                         .AddMoneyField(x => x.PriceWithTax)
                         .AddMoneyField(x => x.Total)
                         .AddMoneyField(x => x.TotalWithTax)
                         .AddMoneyField(x => x.DiscountAmount)
                         .AddMoneyField(x => x.DiscountAmountWithTax)
                         .AddMoneyField(x => x.TaxTotal)
                         .AddField(x => x.TaxPercentRate)
                         .AddField(x => x.TaxType)
                         .AddTaxDetailsField(x => x.TaxDetails)
                         .AddDiscountsField(x => x.Discounts));

            return query;
        }

        public static IQuery<T> AddAddressesField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<Model.Address>>> selector) where T : class
        {
            query.AddField<Model.Address>(
                    selector,
                    t => t.FillAddressField());

            return query;
        }

        public static IQuery<T> AddCouponsField<T>(this IQuery<T> query, Expression<Func<T, IEnumerable<Model.Marketing.Coupon>>> selector) where T : class
        {
            query.AddField<Model.Marketing.Coupon>(
                    selector,
                    t => t.FillCouponField());

            return query;
        }
    }


}
