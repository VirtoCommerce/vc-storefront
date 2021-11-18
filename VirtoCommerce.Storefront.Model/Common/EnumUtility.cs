using System;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class EnumUtility
    {
        public static T SafeParse<T>(string value, T defaultValue)
            where T : struct
        {
            if (!Enum.TryParse(value, out T result))
            {
                result = defaultValue;
            }

            return result;
        }
    }
}
