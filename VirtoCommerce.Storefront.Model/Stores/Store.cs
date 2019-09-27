using System;
using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

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

        public string Host
        {
            get
            {
                string result = null;
                if (!string.IsNullOrEmpty(Url) && Uri.TryCreate(Url, UriKind.Absolute, out var url))
                {
                    result = url.Host;
                }
                return result;
            }
        }

        /// <summary>
        /// State of store (open, closing, maintenance)
        /// </summary>
        public StoreStatus StoreState { get; set; }

        public string TimeZone { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string Status => StoreState.ToString();
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

        public bool QuotesEnabled { get; set; }

        public bool SubscriptionEnabled { get; set; }

        public bool TaxCalculationEnabled { get; set; }

        public bool AnonymousUsersAllowed { get; set; }

        public decimal FixedTaxRate { get; set; }


        public IMutablePagedList<DynamicProperty> DynamicProperties { get; set; }

        #region IHasSettings Members

        public IMutablePagedList<SettingEntry> Settings { get; set; }

        #endregion

        public SeoLinksType SeoLinksType { get; set; }

        public IList<PaymentMethod> PaymentMethods { get; set; }

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
