using VirtoCommerce.Storefront.Model;
using platformDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class SettingConverter
    {
        public static SettingEntry ToSettingEntry(this platformDto.Setting settingDto)
        {
            var retVal = new SettingEntry();
            retVal.DefaultValue = settingDto.DefaultValue;
            retVal.Description = settingDto.Description;
            retVal.IsArray = settingDto.IsArray ?? false;
            retVal.Name = settingDto.Name;
            retVal.Title = settingDto.Title;
            retVal.Value = settingDto.Value;
            retVal.ValueType = settingDto.ValueType;
            retVal.AllowedValues = settingDto.AllowedValues;
            retVal.ArrayValues = settingDto.ArrayValues;
            return retVal;
        }
    }
}
