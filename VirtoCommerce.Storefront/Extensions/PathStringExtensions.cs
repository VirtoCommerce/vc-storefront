using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class PathStringExtensions
    {

        const string FILE_SCHEME = "file";

        private static string _storeLangeExpr = @"^/\b\S+\b/[a-zA-Z]{2}/";
        public static PathString GetStoreAndLangSegment(this PathString path)
        {
            var matches = Regex.Match(path, _storeLangeExpr);
            return matches.Success ? matches.Value : "/";
        }

        public static bool IsApi(this PathString path)
        {
            return path.ToString().Contains("/storefrontapi/");
        }

        public static PathString TrimStoreAndLangSegment(this PathString path, Store store, Language language)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            //need to remove store and language if it already exist in path           
            path = Regex.Replace(path, @"^/\b" + store.Id + @"\b/", "/", RegexOptions.IgnoreCase);
            path = Regex.Replace(path, @"/\b" + language.CultureName + @"\b/", "/", RegexOptions.IgnoreCase);
            return path;
        }

        public static PathString AddStoreAndLangSegment(this PathString path, Store store, Language language)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            var result = new PathString();

            //add store to path
            result = result.Add(new PathString("/" + store.Id));
            //add language to path
            result = result.Add(new PathString("/" + language.CultureName));

            //need to remove store and language if it already exist in the path
            result = result.Add(new PathString(path.TrimStoreAndLangSegment(store, language)));

            return result;
        }

        /// <summary>
        /// Trims first occurrence of store path ("/store" for "http://localhost/store") from the beginning of the url. Does nothing in case of empty store.Url.
        /// </summary>
        /// <param name="path">Path to trim store path from.</param>
        /// <param name="store">Store which path to trim.</param>
        /// <returns></returns>
        public static PathString TrimStorePath(this PathString path, Store store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            // Need to remove store path only if store has URL
            var storeUrl = !string.IsNullOrWhiteSpace(store.Url) ? store.Url : store.SecureUrl;

            if (!string.IsNullOrWhiteSpace(storeUrl) && (Uri.TryCreate(storeUrl, UriKind.Absolute, out var storeUri) && storeUri.Scheme != FILE_SCHEME))
            {
                var storeUriPath = storeUri.AbsolutePath.Trim('/');

                // Uri.AbsolutePath by default is "/" - no need to trim it
                if (!string.IsNullOrWhiteSpace(storeUriPath) && !storeUriPath.Equals("/"))
                {
                    // Removing store url path from the beginning of path
                    path = Regex.Replace(path, @"^/\b" + storeUriPath + @"\b/", "/", RegexOptions.IgnoreCase);
                }
            }

            return path;
        }

        public static PathString ToAbsolutePath(this string path)
        {
            // Checks whether path is absolute path (starts with scheme), and extract local path if it is
            if (Uri.TryCreate(path, UriKind.Absolute, out var absoluteUri) && absoluteUri.Scheme != FILE_SCHEME)
            {
                return absoluteUri.PathAndQuery;
            }

            return path;
        }
    }
}
