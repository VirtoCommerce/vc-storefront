using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Inventory;

namespace VirtoCommerce.Storefront.Model.Stores
{
    /// <summary>
    /// Represent store - main ecommerce aggregate unit
    /// </summary>
    public partial class Store : Entity, IHasSettings
    {
        public Store()
        {
            Languages = new List<Language>();
            CurrenciesCodes = new List<string>();
            SeoInfos = new List<SeoInfo>();
            DynamicProperties = new List<DynamicProperty>();
            Settings = new List<SettingEntry>();
            AvailFulfillmentCenterIds = new List<string>();
            TrustedGroups = new List<string>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Url of store storefront
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Secure url of store, must use https protocol, required
        /// </summary>
        public string SecureUrl { get; set; }

        /// <summary>
        /// State of store (open, closing, maintenance)
        /// </summary>
        public StoreStatus StoreState { get; set; }

        public string TimeZone { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        /// <summary>
        /// Default Language culture name  of store ( example en-US )
        /// </summary>
        public Language DefaultLanguage { get; set; }
        /// <summary>
        /// All supported languages
        /// </summary>
        public IList<Language> Languages { get; set; }

        /// <summary>
        /// Default currency of store. 
        /// Achtung ! Do not use Currency objects here, because Store object can be cached but Currency may be changed depend on request culture.
        /// </summary>
        public string DefaultCurrencyCode { get; set; }
        /// <summary>
        /// List of all supported currencies codes
        /// </summary>
        public IList<string> CurrenciesCodes { get; set; }

        /// <summary>
        /// Product catalog id assigned to store
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// Contact email of store
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Administrator contact email of store
        /// </summary>
        public string AdminEmail { get; set; }

        /// <summary>
        /// Store theme name
        /// </summary>
        public string ThemeName { get; set; }

        public IList<DynamicProperty> DynamicProperties { get; set; }

        /// <summary>
        /// All store seo informations for all languages
        /// </summary>
        public IList<SeoInfo> SeoInfos { get; set; }

        /// <summary>
        /// All linked stores (their accounts can be reused here)
        /// </summary>
        public IList<string> TrustedGroups { get; set; }

        /// <summary>
        /// Main (default)  fulfillment center
        /// </summary>
        public string DefaultFulfillmentCenterId { get; set; }

        /// <summary>
        /// Additional fulfillment centers
        /// </summary>
        public IList<string> AvailFulfillmentCenterIds { get; set; }

        public bool QuotesEnabled
        {
            get
            {
                return Settings.GetSettingValue("Quotes.EnableQuotes", false);
            }
        }

        public bool SubscriptionEnabled
        {
            get
            {
                return Settings.GetSettingValue("Subscription.EnableSubscriptions", false);
            }
        }

        public bool TaxCalculationEnabled
        {
            get
            {
                return Settings.GetSettingValue("Stores.TaxCalculationEnabled", true);
            }
        }

        public decimal FixedTaxRate { get; set; }

        #region IHasSettings Members

        public IList<SettingEntry> Settings { get; set; }

        #endregion

        public SeoLinksType SeoLinksType { get; set; }

        /// <summary>
        /// Checks if specified URL starts with store URL or store secure URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsStoreUrl(Uri url)
        {
            var result = false;

            var requestAddress = url.ToString();

            if (!string.IsNullOrEmpty(Url))
            {
                result = requestAddress.StartsWith(Url, StringComparison.InvariantCultureIgnoreCase);
            }

            if (!result && !string.IsNullOrEmpty(SecureUrl))
            {
                result = requestAddress.StartsWith(SecureUrl, StringComparison.InvariantCultureIgnoreCase);
            }

            return result;
        }
    }
}
