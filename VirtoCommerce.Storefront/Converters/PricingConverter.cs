using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Pricing;
using pricingDto = VirtoCommerce.Storefront.AutoRestClients.PricingModuleApi.Models;

namespace VirtoCommerce.Storefront.Converters
{
    public static class PricingConverterExtension
    {
        public static PricingConverter PricingConverterInstance
        {
            get
            {
                return new PricingConverter();
            }
        }

        public static pricingDto.PriceEvaluationContext ToPriceEvaluationContextDto(this WorkContext workContext, IEnumerable<Product> products = null)
        {
            return PricingConverterInstance.ToPriceEvaluationContextDto(workContext, products);
        }
      
        public static ProductPrice ToProductPrice(this pricingDto.Price priceDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            return PricingConverterInstance.ToProductPrice(priceDto, availCurrencies, language);
        }

        public static Pricelist ToPricelist(this pricingDto.Pricelist pricelistDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            return PricingConverterInstance.ToPricelist(pricelistDto, availCurrencies, language);
        }

        public static TierPrice ToTierPrice(this pricingDto.Price priceDto, Currency currency)
        {
            return PricingConverterInstance.ToTierPrice(priceDto, currency);
        }

    }

    public partial class PricingConverter
    {
        public virtual TierPrice ToTierPrice(pricingDto.Price priceDto, Currency currency)
        {
            var listPrice = new Money(priceDto.List ?? 0, currency);

            return new TierPrice(currency)
            {
                Quantity = priceDto.MinQuantity ?? 1,
                Price = priceDto.Sale.HasValue ? new Money(priceDto.Sale.Value, currency) : listPrice
            };
        }

        public virtual Pricelist ToPricelist(pricingDto.Pricelist pricelistDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(pricelistDto.Currency)) ?? new Currency(language, pricelistDto.Currency);
            var result = new Pricelist(currency);
            result.Id = pricelistDto.Id;
            return result;
        }

        public virtual ProductPrice ToProductPrice(pricingDto.Price priceDto, IEnumerable<Currency> availCurrencies, Language language)
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

        public virtual pricingDto.PriceEvaluationContext ToPriceEvaluationContextDto(WorkContext workContext, IEnumerable<Product> products = null)
        {

            //Evaluate products prices
            var retVal = new pricingDto.PriceEvaluationContext
            {
                PricelistIds = workContext.CurrentPricelists.Select(p => p.Id).ToList(),
                CatalogId = workContext.CurrentStore.Catalog,
                Language = workContext.CurrentLanguage.CultureName,
                CertainDate = workContext.StorefrontUtcNow,
                StoreId = workContext.CurrentStore.Id
            };

            if (workContext.CurrentCustomer != null)
            {
                retVal.CustomerId = workContext.CurrentCustomer.Id;
                retVal.GeoTimeZone = workContext.CurrentCustomer.TimeZone;
                var address = workContext.CurrentCustomer.DefaultShippingAddress ?? workContext.CurrentCustomer.DefaultBillingAddress;
                if (address != null)
                {
                    retVal.GeoCity = address.City;
                    retVal.GeoCountry = address.CountryCode;
                    retVal.GeoState = address.RegionName;
                    retVal.GeoZipCode = address.PostalCode;
                }
                if (workContext.CurrentCustomer.UserGroups != null)
                {
                    retVal.UserGroups = workContext.CurrentCustomer.UserGroups.ToList();
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
