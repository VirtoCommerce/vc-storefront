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
        public string Language { get; set; }
        public string[] AvailLanguages { get; set; }
        public string CatalogId { get; set; }
        public string Currency { get; set; }
        public string[] AvailCurrencies { get; set; }
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
                Language = workContext.CurrentLanguage?.CultureName,
                AvailLanguages = workContext.CurrentStore?.Languages?.Select(x => x.CultureName).ToArray(),
                CatalogId = workContext.CurrentStore?.Catalog,
                Currency = workContext.CurrentCurrency?.Code,
                AvailCurrencies = workContext.CurrentStore?.CurrenciesCodes.ToArray(),
                UserId = workContext.CurrentUser?.Id,
                UserName = workContext.CurrentUser?.Name,
                Settings = workContext.Settings
            };
            return result;
        }
    }
}
