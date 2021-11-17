using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class LocalizationExtensions
    {
        private static readonly RegionInfo[] _cachedRegionInfos;
        static LocalizationExtensions()
        {
            _cachedRegionInfos = CultureInfo.GetCultures(CultureTypes.AllCultures)
                                            .Where(c => !c.IsNeutralCulture && c.LCID != 127)
                .Select(x =>
                {
                    try
                    {
                        return new RegionInfo(x.LCID);
                    }
                    catch
                    {
                        return null;
                    }
                }).Where(x => x != null).ToArray();
        }

        public static T FindWithLanguage<T>(this IEnumerable<T> items, Language language)
         where T : IHasLanguage
        {
            var retVal = items.FirstOrDefault(i => i.Language.Equals(language));
            if (retVal == null)
            {
                retVal = items.FirstOrDefault(x => x.Language.IsInvariant);
            }
            return retVal;
        }

        public static TValue FindWithLanguage<T, TValue>(this IEnumerable<T> items, Language language, Func<T, TValue> valueGetter, TValue defaultValue) where T : IHasLanguage
        {
            var retVal = defaultValue;
            var item = items.OfType<IHasLanguage>().FindWithLanguage(language);
            if (item != null)
            {
                retVal = valueGetter((T)item);
            }
            return retVal;
        }

        /// <summary>
        /// Return all localized strings for specified language also always returns strings with invariant language
        /// </summary>
        /// <param name="localizedStrings"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static IEnumerable<LocalizedString> GetLocalizedStringsForLanguage(this IEnumerable<LocalizedString> localizedStrings, Language language)
        {
            if (localizedStrings == null)
            {
                throw new ArgumentNullException("localizedStrings");
            }
            if (language == null)
            {
                throw new ArgumentNullException("language");
            }
            var retVal = localizedStrings.Where(x => x.Language == language || x.Language.IsInvariant).ToArray();
            return retVal;
        }

        public static string GetCurrencySymbol(this string ISOCurrencySymbol)
        {
            var symbol = _cachedRegionInfos.Where(x => x != null && String.Equals(x.ISOCurrencySymbol, ISOCurrencySymbol, StringComparison.InvariantCultureIgnoreCase))
                                       .Select(ri => ri.CurrencySymbol)
                                       .FirstOrDefault();
            return symbol;
        }
    }
}
