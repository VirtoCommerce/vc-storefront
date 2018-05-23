using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using platformDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class DynamicPropertyConverter
    {
        public static DynamicProperty ToDynamicProperty(this coreDto.DynamicObjectProperty propertyDto)
        {
            var result = new DynamicProperty();

            result.Id = propertyDto.Id;
            result.IsArray = propertyDto.IsArray ?? false;
            result.IsDictionary = propertyDto.IsDictionary ?? false;
            result.IsRequired = propertyDto.IsRequired ?? false;
            result.Name = propertyDto.Name;
            result.ValueType = propertyDto.ValueType;

            if (propertyDto.DisplayNames != null)
            {
                result.DisplayNames = propertyDto.DisplayNames.Select(x => new LocalizedString(new Language(x.Locale), x.Name)).ToList();
            }

            if (propertyDto.Values != null)
            {
                if (result.IsDictionary)
                {
                    var dictValues = propertyDto.Values
                        .Where(x => x.Value != null)
                        .Select(x => x.Value)
                        .Cast<JObject>()
                        .Select(x => x.ToObject<platformDto.DynamicPropertyDictionaryItem>())
                        .ToArray();

                    result.DictionaryValues = dictValues.Select(x => x.ToDictItem()).ToList();
                }
                else
                {
                    result.Values = propertyDto.Values
                        .Where(x => x.Value != null)
                        .Select(x => x.ToLocalizedString())
                        .ToList();
                }
            }

            return result;
        }


        public static coreDto.DynamicObjectProperty ToDynamicPropertyDto(this DynamicProperty dynamicProperty)
        {
            var result = new coreDto.DynamicObjectProperty();

            result.Id = dynamicProperty.Id;
            result.IsArray = dynamicProperty.IsArray;
            result.IsDictionary = dynamicProperty.IsDictionary;
            result.IsRequired = dynamicProperty.IsRequired;
            result.Name = dynamicProperty.Name;
            result.ValueType = dynamicProperty.ValueType;

            if (dynamicProperty.Values != null)
            {
                result.Values = dynamicProperty.Values.Select(v => v.ToPropertyValueDto()).ToList();
            }
            else if (dynamicProperty.DictionaryValues != null)
            {
                result.Values = dynamicProperty.DictionaryValues.Select(x => x.ToPropertyValueDto()).ToList();
            }

            return result;
        }

        private static DynamicPropertyDictionaryItem ToDictItem(this platformDto.DynamicPropertyDictionaryItem dto)
        {
            var result = new DynamicPropertyDictionaryItem();
            result.Id = dto.Id;
            result.Name = dto.Name;
            result.PropertyId = dto.PropertyId;
            if (dto.DisplayNames != null)
            {
                result.DisplayNames = dto.DisplayNames.Select(x => new LocalizedString(new Language(x.Locale), x.Name)).ToList();
            }
            return result;
        }

        private static LocalizedString ToLocalizedString(this coreDto.DynamicPropertyObjectValue dto)
        {
            return new LocalizedString(new Language(dto.Locale), string.Format(CultureInfo.InvariantCulture, "{0}", dto.Value));
        }

        private static coreDto.DynamicPropertyObjectValue ToPropertyValueDto(this DynamicPropertyDictionaryItem dictItem)
        {
            var result = new coreDto.DynamicPropertyObjectValue { Value = dictItem };
            return result;
        }

        private static coreDto.DynamicPropertyObjectValue ToPropertyValueDto(this LocalizedString dynamicPropertyObjectValue)
        {
            var result = new coreDto.DynamicPropertyObjectValue
            {
                Value = dynamicPropertyObjectValue.Value,
                Locale = dynamicPropertyObjectValue.Language.CultureName
            };

            return result;
        }
    }
}
