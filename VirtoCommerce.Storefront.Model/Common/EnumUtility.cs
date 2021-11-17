using System;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class EnumUtility
    {
        public static T SafeParse<T>(string value, T defaultValue)
            where T : struct
        {
            T result;

            if (!Enum.TryParse(value, out result))
                result = defaultValue;

            return result;
        }
    }
}
