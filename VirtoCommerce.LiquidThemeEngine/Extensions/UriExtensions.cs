using System;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Extensions
{
    public static class UriExtensions
    {
        /// <summary>
        /// Sets the given parameter value in the query string.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name">Name of the parameter to set.</param>
        /// <param name="value">Value for the parameter to set. Pass null to remove the parameter with given name.</param>
        /// <returns>Url with given parameter value.</returns>
        public static Uri SetQueryParameter(this Uri url, string name, string value)
        {
            var qs = HttpUtility.ParseQueryString(url.Query);

            if (value != null)
            {
                qs[name] = value;
            }
            else
            {
                qs.Remove(name);
            }

            var result = new UriBuilder(url)
            {
                Query = string.Join("&", qs.AllKeys.Select(key => string.Join("=", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(qs[key]))))
            };

            return result.Uri;
        }

        public static Uri SetQueryParameters(this Uri uri, IHasQueryKeyValues hasQueryKeyValues)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            if (hasQueryKeyValues == null)
            {
                throw new ArgumentNullException(nameof(hasQueryKeyValues));
            }

            foreach (var keyValue in hasQueryKeyValues.GetQueryKeyValues())
            {
                uri = uri.SetQueryParameter(keyValue.Key, keyValue.Value);
            }
            return uri;
        }
    }
}
