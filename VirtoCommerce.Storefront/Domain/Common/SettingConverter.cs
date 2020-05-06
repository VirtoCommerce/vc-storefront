using System.Linq;
using VirtoCommerce.Storefront.Model;
using platformDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class SettingConverter
    {
        public static SettingEntry ToSettingEntry(this platformDto.ObjectSettingEntry settingDto)
        {
            var retVal = new SettingEntry
            {
                DefaultValue = settingDto.DefaultValue,
                IsArray = false,
                Name = settingDto.Name,
                Value = settingDto.Value,
                ValueType = settingDto.ValueType
            };
            if (settingDto.AllowedValues != null)
            {
                retVal.AllowedValues = settingDto.AllowedValues.ToList();
            }
            return retVal;
        }
    }
}
