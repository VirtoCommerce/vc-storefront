using System.Linq;
using VirtoCommerce.Storefront.Model;
using platformDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class SettingConverter
    {
        public static SettingEntry ToSettingEntry(this platformDto.ObjectSettingEntry settingDto)
        {
            var retVal = new SettingEntry();
            retVal.DefaultValue = settingDto.DefaultValue?.ToString();
            retVal.IsArray = false;
            retVal.Name = settingDto.Name;
            retVal.Value = settingDto.Value?.ToString();
            retVal.ValueType = settingDto.ValueType;
            if (settingDto.AllowedValues != null)
            {
                retVal.AllowedValues = settingDto.AllowedValues.Cast<string>().ToList();
            }
            return retVal;
        }
    }
}
