using System;
using System.Collections.Specialized;
using System.Linq;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class NameValueCollectionExtensions
    {
        [Obsolete("Use ConfigurationHelper.GetAppSettingsValue()")]
        [CLSCompliant(false)]
        public static T GetValue<T>(this NameValueCollection nameValuePairs, string configKey, T defaultValue)
                where T : IConvertible
        {
            T result;

            if (nameValuePairs.AllKeys.Contains(configKey))
            {
                var tmpValue = nameValuePairs[configKey];

                result = (T)Convert.ChangeType(tmpValue, typeof(T));
            }
            else
            {
                result = defaultValue;
            }

            return result;
        }
    }
}
