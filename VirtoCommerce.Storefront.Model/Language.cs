using System.Collections.Generic;
using System.Globalization;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    /// <summary>
    /// Represent language for specified culture
    /// </summary>
    public partial class Language : ValueObject
    {
        private Language()
            : this(CultureInfo.InvariantCulture.Name)
        {
        }

        public Language(string cultureName)
        {
            var culture = CultureInfo.InvariantCulture;
            if (!string.IsNullOrEmpty(cultureName))
            {
                culture = CultureInfo.GetCultureInfo(cultureName);
            }

            CultureName = culture.Name;
            ThreeLetterLanguageName = culture.ThreeLetterISOLanguageName;
            TwoLetterLanguageName = culture.TwoLetterISOLanguageName;
            NativeName = culture.NativeName;
            if (!culture.IsNeutralCulture && culture != CultureInfo.InvariantCulture)
            {
                var regionInfo = new RegionInfo(cultureName);
                TwoLetterRegionName = regionInfo.TwoLetterISORegionName;
                ThreeLetterRegionName = regionInfo.ThreeLetterISORegionName;
            }
        }

        public static Language InvariantLanguage
        {
            get
            {
                return new Language();
            }
        }

        public bool IsInvariant
        {
            get
            {
                return CultureName == CultureInfo.InvariantCulture.Name;
            }
        }
        /// <summary>
        /// culture name format (e.g. en-US)
        /// </summary>
        public string CultureName { get; private set; }
        public string NativeName { get; private set; }
        /// <summary>
        ///  Gets the ISO 639-2 three-letter code for the language 
        /// </summary>
        public string ThreeLetterLanguageName { get; private set; }
        /// <summary>
        ///   Gets the ISO 639-1 two-letter code for the language
        /// </summary>
        public string TwoLetterLanguageName { get; private set; }
        /// <summary>
        ///  Gets the two-letter code defined in ISO 3166 for the country/region.
        /// </summary>
        public string TwoLetterRegionName { get; private set; }
        /// <summary>
        ///  Gets the three-letter code defined in ISO 3166 for the country/region.
        /// </summary>
        public string ThreeLetterRegionName { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CultureName;
        }

    }
}
