using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using GraphQL.Query.Builder;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common;

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

        public static IQuery<T> AddMoneyField<T>(this IQuery<T> query, Expression<Func<T, MoneyDto>> selector) where T : class
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

        public static IQuery<T> AddAddressField<T>(this IQuery<T> query, Expression<Func<T, CartAddressDto>> selector) where T : class
        {
            query.AddField(
                selector,
                t => t
                    .AddField(x => x.Key)
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
                    .AddField(x => x.Email));

            return query;
        }

        public static IQuery<T> AddCurrencyField<T>(this IQuery<T> query, Expression<Func<T, CurrencyDto>> selector) where T : class
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

        public static IQuery<T> AddTaxDetailField<T>(this IQuery<T> query, Expression<Func<T, TaxDetailDto>> selector) where T : class
        {
            query.AddField(
                    selector,
                    t => t
                        .AddMoneyField(v => v.Rate)
                        .AddMoneyField(v => v.Amount)
                        //.AddField(v => v.Name) //TODO: fix "GraphQL.Validation.ValidationError: Field name of type MoneyType must have a sub selection"
                        //.AddField(v => v.Price)) //TODO: fix "GraphQL.Validation.ValidationError: Field price of type MoneyType must have a sub selection"
                        );

            return query;
        }
    }
}
