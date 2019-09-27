using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class ObjectExtensions
    {
        public static void CopyFrom(this object obj, NameValueCollection source)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            var objType = obj.GetType();

            foreach (var key in source.AllKeys)
            {
                var prop = objType.GetProperty(key);
                if (prop != null)
                {
                    if (prop.PropertyType.IsEnum)
                    {
                        prop.SetValue(obj, Enum.Parse(prop.PropertyType, source[key]), null);
                    }
                    else
                    {
                        var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                        if (typeConverter != null)
                        {
                            var propValue = typeConverter.ConvertFromString(source[key]);
                            prop.SetValue(obj, propValue, null);
                        }
                    }
                }
            }
        }
    }
}
