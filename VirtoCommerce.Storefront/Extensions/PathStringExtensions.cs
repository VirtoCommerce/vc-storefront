using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class PathStringExtensions 
    {
        private static Regex _storeLangeExpr = new Regex(@"^/\b\S+\b/[a-zA-Z]{2}-[a-zA-Z]{2}/");
        public static PathString GetStoreAndLangSegment(this PathString path)
        {       
            var matches = _storeLangeExpr.Match(path);
            return matches.Success ? matches.Value : "/";
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
    }
}
