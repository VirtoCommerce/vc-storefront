using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public static class SelectCurrentLanguagePolicy
    {
        public static Language GetCurrentLanguage(this HttpContext context, Store store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            //Try to get language from request
            var result = store.DefaultLanguage;
            var regexpPattern = string.Format(@"\/({0})\/?", string.Join("|", store.Languages.Select(x => x.CultureName)));
            var match = Regex.Match(context.Request.Path, regexpPattern, RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count > 1)
            {
                var language = new Language(match.Groups[1].Value);
                //Get store default language if language not in the supported by stores list
                result = store.Languages.Contains(language) ? language : result;
            }

            return result;
        }
    }
}
