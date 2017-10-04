using System;
using System.Collections.Generic;
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
    public static class StoreConverterExtension
    {
        public static StoreConverter StoreConverterInstance
        {
            get
            {
                return new StoreConverter();
            }
        }

        public static Store ToStore(this storeDto.Store storeDto)
        {
            return StoreConverterInstance.ToStore(storeDto);
        }

        public static DynamicProperty ToDynamicProperty(this storeDto.DynamicObjectProperty propertyDto)
        {
            return StoreConverterInstance.ToDynamicProperty(propertyDto);
        }

        public static SeoInfo ToSeoInfo(this storeDto.SeoInfo seoDto)
        {
            return StoreConverterInstance.ToSeoInfo(seoDto);
        }
    }

    public partial class StoreConverter
    {
        public virtual SeoInfo ToSeoInfo(storeDto.SeoInfo seoDto)
        {
            return seoDto.JsonConvert<coreDto.SeoInfo>().ToSeoInfo();
        }

        public virtual VirtoCommerce.Storefront.Model.Inventory.FulfillmentCenter ToFulfillmentCenter(storeDto.FulfillmentCenter fulfillmentDto)
        {
            var result = new VirtoCommerce.Storefront.Model.Inventory.FulfillmentCenter
            {
                Id = fulfillmentDto.Id,
                Name = fulfillmentDto.Name
            };
            return result;
        }

        public virtual DynamicProperty ToDynamicProperty(storeDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<coreDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public virtual Store ToStore(storeDto.Store storeDto)
        {
            var result = new Store();
            result.AdminEmail = storeDto.AdminEmail;
            result.Catalog = storeDto.Catalog;
            result.Country = storeDto.Country;
            result.Description = storeDto.Description;
            result.Email = storeDto.Email;
            result.Id = storeDto.Id;
            result.Name = storeDto.Name;
            result.Region = storeDto.Region;
            result.SecureUrl = storeDto.SecureUrl;
            result.TimeZone = storeDto.TimeZone;
            result.Url = storeDto.Url;

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
                result.DynamicProperties = storeDto.DynamicProperties.Select(ToDynamicProperty).ToList();
                result.ThemeName = result.DynamicProperties.GetDynamicPropertyValue("DefaultThemeName");
            }

            if (!storeDto.Settings.IsNullOrEmpty())
            {
                result.Settings = storeDto.Settings.Where(x => !x.ValueType.EqualsInvariant("SecureString")).Select(x => x.JsonConvert<platformDto.Setting>().ToSettingEntry()).ToList();
            }

            if (storeDto.FulfillmentCenter != null)
            {
                result.PrimaryFullfilmentCenter = ToFulfillmentCenter(storeDto.FulfillmentCenter);
                result.FulfilmentCenters.Add(result.PrimaryFullfilmentCenter);
            }
            if (!storeDto.FulfillmentCenters.IsNullOrEmpty())
            {
                result.FulfilmentCenters.AddRange(storeDto.FulfillmentCenters.Select(x => ToFulfillmentCenter(x)));
            }

            result.TrustedGroups = storeDto.TrustedGroups;
            result.StoreState = EnumUtility.SafeParse(storeDto.StoreState, StoreStatus.Open);
            result.SeoLinksType = EnumUtility.SafeParse(result.Settings.GetSettingValue("Stores.SeoLinksType", ""), SeoLinksType.Collapsed);

            return result;
        }
    }
}
