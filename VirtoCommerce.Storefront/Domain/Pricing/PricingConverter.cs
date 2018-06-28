using System;
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


        public static PriceEvaluationContext ToPriceEvaluationContext(this WorkContext workContext, IEnumerable<Pricelist> pricelists, IEnumerable<Product> products = null)
        {
            if (workContext == null)
            {
                throw new ArgumentNullException(nameof(workContext));
            }

            //Evaluate products prices
            var result = new PriceEvaluationContext
            {

                CatalogId = workContext.CurrentStore.Catalog,
                Language = workContext.CurrentLanguage.CultureName,
                StoreId = workContext.CurrentStore.Id
            };

            if (workContext.CurrentUser != null)
            {
                result.CustomerId = workContext.CurrentUser.Id;
                var contact = workContext.CurrentUser?.Contact;

                if (contact != null)
                {
                    result.GeoTimeZone = contact.TimeZone;
                    var address = contact.DefaultShippingAddress ?? contact.DefaultBillingAddress;
                    if (address != null)
                    {
                        result.GeoCity = address.City;
                        result.GeoCountry = address.CountryCode;
                        result.GeoState = address.RegionName;
                        result.GeoZipCode = address.PostalCode;
                    }
                    if (contact.UserGroups != null)
                    {
                        result.UserGroups = contact.UserGroups;
                    }
                }
            }
            if (pricelists != null)
            {
                result.PricelistIds = pricelists.Select(p => p.Id).ToList();
            }
            if (products != null)
            {
                result.ProductIds = products.Select(p => p.Id).ToList();
            }
            return result;
        }

    }
}
