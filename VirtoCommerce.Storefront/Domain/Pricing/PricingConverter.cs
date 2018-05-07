using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Pricing;
using pricingDto = VirtoCommerce.Storefront.AutoRestClients.PricingModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
   
    public static partial class PricingConverter
    {
        public static TierPrice ToTierPrice(this pricingDto.Price priceDto, Currency currency)
        {
            var listPrice = new Money(priceDto.List ?? 0, currency);

            return new TierPrice(currency)
            {
                Quantity = priceDto.MinQuantity ?? 1,
                Price = priceDto.Sale.HasValue ? new Money(priceDto.Sale.Value, currency) : listPrice
            };
        }

        public static Pricelist ToPricelist(this pricingDto.Pricelist pricelistDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(pricelistDto.Currency)) ?? new Currency(language, pricelistDto.Currency);
            var result = new Pricelist(currency);
            result.Id = pricelistDto.Id;
            return result;
        }

        public static ProductPrice ToProductPrice(this pricingDto.Price priceDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(priceDto.Currency)) ?? new Currency(language, priceDto.Currency);
            var result = new ProductPrice(currency);
            result.ProductId = priceDto.ProductId;
            result.PricelistId = priceDto.PricelistId;
           
            result.Currency = currency;
            result.ListPrice = new Money(priceDto.List ?? 0d, currency);
            result.SalePrice = priceDto.Sale == null ? result.ListPrice : new Money(priceDto.Sale ?? 0d, currency);
            result.MinQuantity = priceDto.MinQuantity;
            return result;
        }

        public static pricingDto.PriceEvaluationContext ToPriceEvaluationContextDto(this PriceEvaluationContext evalContext)
        {
            return evalContext.JsonConvert<pricingDto.PriceEvaluationContext>();
        }

        public static PriceEvaluationContext ToPriceEvaluationContext(this WorkContext workContext, IEnumerable<Product> products = null)
        {
            //Evaluate products prices
            var retVal = new PriceEvaluationContext
            {
                PricelistIds = workContext.CurrentPricelists.Select(p => p.Id).ToList(),
                CatalogId = workContext.CurrentStore.Catalog,
                Language = workContext.CurrentLanguage.CultureName,
                StoreId = workContext.CurrentStore.Id
            };

            if (workContext.CurrentUser != null)
            {
                retVal.CustomerId = workContext.CurrentUser.Id;
                var contact = workContext.CurrentUser?.Contact;

                if (contact != null)
                {
                    retVal.GeoTimeZone = contact.TimeZone;
                    var address = contact.DefaultShippingAddress ?? contact.DefaultBillingAddress;
                    if (address != null)
                    {
                        retVal.GeoCity = address.City;
                        retVal.GeoCountry = address.CountryCode;
                        retVal.GeoState = address.RegionName;
                        retVal.GeoZipCode = address.PostalCode;
                    }
                    if (contact.UserGroups != null)
                    {
                        retVal.UserGroups = contact.UserGroups;
                    }
                }
            }

            if (products != null)
            {
                retVal.ProductIds = products.Select(p => p.Id).ToList();
            }
            return retVal;
        }
      
    }
}
