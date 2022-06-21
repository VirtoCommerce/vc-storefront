using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public class SpaThemeContext
    {
        public string BaseUrl { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public Language DefaultLanguage { get; set; }
        public Language[] AvailLanguages { get; set; }
        public string CatalogId { get; set; }
        public Currency DefaultCurrency { get; set; }
        public Currency[] AvailCurrencies { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IDictionary<string, object> Settings { get; set; }

        public static SpaThemeContext Create(WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new SpaThemeContext
            {
                BaseUrl = urlBuilder.ToAppAbsolute("/"),
                StoreId = workContext.CurrentStore?.Id,
                StoreName = workContext.CurrentStore?.Name,
                DefaultLanguage = workContext.CurrentStore?.DefaultLanguage,
                AvailLanguages = workContext.CurrentStore?.Languages.ToArray(),
                CatalogId = workContext.CurrentStore?.Catalog,
                DefaultCurrency = workContext.AllCurrencies.FirstOrDefault(x => x.Code == workContext.CurrentStore.DefaultCurrencyCode),
                AvailCurrencies = workContext.AllCurrencies.ToArray(),
                UserId = workContext.CurrentUser?.Id,
                UserName = workContext.CurrentUser?.Name,
                Settings = workContext.Settings
            };
            return result;
        }
    }
}
