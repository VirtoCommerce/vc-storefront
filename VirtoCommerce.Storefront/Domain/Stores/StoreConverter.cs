using System;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using platformDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using storeDto = VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{

    public static partial class StoreConverter
    {
        public static SeoInfo ToSeoInfo(this storeDto.SeoInfo seoDto)
        {
            return seoDto.JsonConvert<coreDto.SeoInfo>().ToSeoInfo();
        }


        public static DynamicProperty ToDynamicProperty(this storeDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<platformDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public static Store ToStore(this storeDto.Store storeDto)
        {
            var result = new Store
            {
                AdminEmail = storeDto.AdminEmail,
                Catalog = storeDto.Catalog,
                Country = storeDto.Country,
                Description = storeDto.Description,
                Email = storeDto.Email,
                Id = storeDto.Id,
                Name = storeDto.Name,
                Region = storeDto.Region,
                SecureUrl = storeDto.SecureUrl,
                TimeZone = storeDto.TimeZone,
                Url = storeDto.Url,
                DefaultFulfillmentCenterId = storeDto.MainFulfillmentCenterId,
                AvailFulfillmentCenterIds = (storeDto.AdditionalFulfillmentCenterIds ?? Array.Empty<string>()).ToList(),
            };

            if (result.DefaultFulfillmentCenterId != null)
            {
                result.AvailFulfillmentCenterIds.Add(result.DefaultFulfillmentCenterId);
            }

            if (!storeDto.SeoInfos.IsNullOrEmpty())
            {
                result.SeoInfos = storeDto.SeoInfos.Select(ToSeoInfo).ToList();
            }

            result.DefaultLanguage = storeDto.DefaultLanguage != null ? new Language(storeDto.DefaultLanguage) : Language.InvariantLanguage;

            if (!storeDto.Languages.IsNullOrEmpty())
            {
                result.Languages = storeDto.Languages.Select(x => new Language(x)).ToList();
            }

            result.CurrenciesCodes = storeDto.Currencies.Concat(new[] { storeDto.DefaultCurrency })
                                                   .Where(x => !string.IsNullOrEmpty(x))
                                                   .Distinct()
                                                   .ToList();
            result.DefaultCurrencyCode = storeDto.DefaultCurrency;

            if (!storeDto.DynamicProperties.IsNullOrEmpty())
            {
                result.DynamicProperties = new MutablePagedList<DynamicProperty>(storeDto.DynamicProperties.Select(ToDynamicProperty).ToList());
                result.ThemeName = result.DynamicProperties.GetDynamicPropertyValue("DefaultThemeName");
            }

            if (!storeDto.Settings.IsNullOrEmpty())
            {
                result.Settings = new MutablePagedList<SettingEntry>(storeDto.Settings.Where(x => !x.ValueType.EqualsInvariant("SecureString"))
                                                                                      .Select(x => x.JsonConvert<platformDto.ObjectSettingEntry>()
                                                                                      .ToSettingEntry()));
            }

            result.TrustedGroups = storeDto.TrustedGroups;
            result.StoreState = EnumUtility.SafeParse(storeDto.StoreState, StoreStatus.Open);
            result.SeoLinksType = EnumUtility.SafeParse(result.Settings.GetSettingValue("Stores.SeoLinksType", ""), SeoLinksType.Collapsed);
            result.QuotesEnabled = result.Settings.GetSettingValue("Quotes.EnableQuotes", false);
            result.SubscriptionEnabled = result.Settings.GetSettingValue("Subscription.EnableSubscriptions", false);
            result.TaxCalculationEnabled = result.Settings.GetSettingValue("Stores.TaxCalculationEnabled", true);
            result.AnonymousUsersAllowed = result.Settings.GetSettingValue("Stores.AllowAnonymousUsers", true);
            result.IsSpa = result.Settings.GetSettingValue("Stores.IsSpa", false);
            result.EmailVerificationEnabled = result.Settings.GetSettingValue("Stores.EmailVerificationEnabled", false);
            result.CreateAnonymousOrderEnabled = result.Settings.GetSettingValue("XOrder.CreateAnonymousOrderEnabled", true);

            result.CartValidationRuleSet = result.Settings.GetSettingValue<string>("Stores.CartValidationRuleSet", null);
            if (string.IsNullOrEmpty(result.CartValidationRuleSet))
            {
                result.CartValidationRuleSet = result.DynamicProperties?.GetDynamicPropertyValue("CartValidationRuleSet");
            }

            return result;
        }


    }
}
