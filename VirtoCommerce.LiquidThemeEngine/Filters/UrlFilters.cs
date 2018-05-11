using System;
using System.Globalization;
using System.Linq;
using System.Web;
using DotLiquid;
using VirtoCommerce.LiquidThemeEngine.Extensions;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;
using shopifyModel = VirtoCommerce.LiquidThemeEngine.Objects;
using storefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/filters/url-filters
    /// </summary>
    public class UrlFilters
    {
        /// <summary>
        /// Generates a link to the customer login page.
        /// {{ 'Log in' | customer_login_link }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CustomerLoginLink(string input)
        {
            return BuildHtmlLink("customer_login_link", "~/account/login", input);
        }
        public static string CustomerRegisterLink(string input)
        {
            return BuildHtmlLink("customer_register_link", "~/account/register", input);
        }

        public static string CustomerLogoutLink(string input)
        {
            return BuildHtmlLink("customer_logout_link", "~/account/logout", input);
        }

        public static string EditCustomerAddressLink(string input, string id)
        {
            return BuildOnClickLink(input, "Shopify.CustomerAddress.toggleForm('{0}');return false", id);
        }

        public static string DeleteCustomerAddressLink(string input, string id)
        {
            return BuildOnClickLink(input, "Shopify.CustomerAddress.destroy('{0}');return false", id);
        }

        /// <summary>
        /// Returns the URL of an image. Accepts an image size as a parameter. The img_url filter can be used on the following objects:
        /// product, variant,  line item, collection, image
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ImgUrl(object input, string type = null)
        {
            if (input == null)
                return null;

            var retVal = input.ToString();
       
            if (input is shopifyModel.Product product)
            {
                retVal = product.FeaturedImage?.Src;
            }
            if (input is shopifyModel.Image image)
            {
                retVal = image.Src;
            }
            if (input is shopifyModel.Variant variant)
            {
                retVal = variant.FeaturedImage?.Src;
            }
            if (input is shopifyModel.Collection collection)
            {
                retVal = collection.Image?.Src;
            }
            if(input is shopifyModel.LineItem lineItem)
            {
                retVal = lineItem.Image?.Src;
            }

            if (!string.IsNullOrEmpty(retVal))
            {
                if (!string.IsNullOrEmpty(type))
                {
                    retVal = retVal.AddSuffixToFileUrl(string.Format("_{0}", type));
                }

                retVal = retVal.RemoveLeadingUriScheme();
            }

            return retVal;
        }

        /// <summary>
        /// Generates an HTML link. The first parameter is the URL of the link, and the optional second parameter is the title of the link.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="link"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string LinkTo(object input, string link, string title = "")
        {
            return string.Format("<a href=\"{0}\" title=\"{1}\">{2}</a>", link, title, input);
        }

        /// <summary>
        /// Creates a link to all products in a collection that have a given tag.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string LinkToTag(Context context, object input, object tag)
        {
            return BuildTagLink(TagAction.Replace, tag, input);
        }

        /// <summary>
        /// Creates a link to all products in a collection that have a given tag as well as any tags that have been already selected.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string LinkToAddTag(Context context, object input, object tag)
        {
            return BuildTagLink(TagAction.Add, tag, input);
        }

        /// <summary>
        /// Generates a link to all products in a collection that have all the previous tags that might have been added already, excluding the given tag.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string LinkToRemoveTag(Context context, object input, object tag)
        {
            return BuildTagLink(TagAction.Remove, tag, input);
        }

        /// <summary>
        /// Returns the URL of a file in the "assets" folder of a theme.
        /// {{ 'shop.css' | asset_url }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string AssetUrl(string input)
        {
            string retVal = null;
            if (input != null)
            {
                var themeAdaptor = (ShopifyLiquidThemeEngine)Template.FileSystem;
                retVal = themeAdaptor.GetAssetAbsoluteUrl(input);
            }
            return retVal;
        }

        /// <summary>
        /// Returns the URL of a file in the "assets/static" folder of a theme.
        /// {{ 'shop.css' | static_asset_url }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StaticAssetUrl(string input)
        {
            string retVal = null;
            if (input != null)
            {
                var themeAdaptor = (ShopifyLiquidThemeEngine)Template.FileSystem;
                retVal = themeAdaptor.GetAssetAbsoluteUrl("static/" + input.TrimStart('/'));
            }
            return retVal;
        }

        /// <summary>
        /// Returns the URL of a file.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FileUrl(string input)
        {
            return AssetUrl(input);
        }

        /// <summary>
        /// Method for switching between multiple stores
        /// </summary>
        /// <param name="input"></param>
        /// <param name="storeId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static string StoreAbsoluteUrl(string input, string storeId = null, string languageCode = null)
        {
            var themeAdaptor = (ShopifyLiquidThemeEngine)Template.FileSystem;
            Store store = null;
            if (!string.IsNullOrEmpty(storeId))
            {
                store = themeAdaptor.WorkContext.AllStores.FirstOrDefault(x => string.Equals(x.Id, storeId, StringComparison.InvariantCultureIgnoreCase));
            }
            store = store ?? themeAdaptor.WorkContext.CurrentStore;

            var retVal = AbsoluteUrl(input, storeId, languageCode);

            var isHttps = themeAdaptor.WorkContext.RequestUrl.Scheme == Uri.UriSchemeHttps;
            //If store has defined url need redirect to it
            if (isHttps)
            {
                retVal = String.IsNullOrEmpty(store.SecureUrl) ? retVal : store.SecureUrl;
            }
            else
            {
                retVal = String.IsNullOrEmpty(store.Url) ? retVal : store.Url;
            }
            return retVal;
        }

        public static string FullUrl(string input, string storeId = null, string languageCode = null)
        {
            var absoluteUrl = AbsoluteUrl(input, storeId, languageCode);

            var themeEngine = (ShopifyLiquidThemeEngine)Template.FileSystem;
            var workContext = themeEngine.WorkContext;

            var fullUrl = new Uri(workContext.RequestUrl, absoluteUrl);

            return fullUrl.AbsoluteUri;
        }

        /// <summary>
        /// Get app absolute storefront url with specified store and language
        /// </summary>
        /// <param name="input"></param>
        /// <param name="storeId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static string AbsoluteUrl(string input, string storeId = null, string languageCode = null)
        {
            if (input == null)
                return string.Empty;

            var themeAdaptor = (ShopifyLiquidThemeEngine)Template.FileSystem;
            Store store = null;
            storefrontModel.Language language = null;
            if (!string.IsNullOrEmpty(storeId))
            {
                store = themeAdaptor.WorkContext.AllStores.FirstOrDefault(x => string.Equals(x.Id, storeId, StringComparison.InvariantCultureIgnoreCase));
            }
            store = store ?? themeAdaptor.WorkContext.CurrentStore;

            if (!string.IsNullOrEmpty(languageCode))
            {
                language = store.Languages.FirstOrDefault(x => string.Equals(x.CultureName, languageCode, StringComparison.InvariantCultureIgnoreCase));
            }
            language = language ?? themeAdaptor.WorkContext.CurrentLanguage;

            var retVal = themeAdaptor.UrlBuilder.ToAppAbsolute(input, store, language);
            return retVal;
        }

        public static string ProductImgUrl(object input, string type = null)
        {
            return ImgUrl(input, type);
        }

        public static string Within(string input, object collection)
        {
            return BuildAbsoluteUrl(input);
        }

        /// <summary>
        /// Appends hash of file content as file version to invalidate browser cache when file changed.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string AppendVersion(string input)
        {
            if (input == null)
                return string.Empty;

            var themeEngine = (ShopifyLiquidThemeEngine)Template.FileSystem;
            var basePath = themeEngine.GetAssetAbsoluteUrl("");
            var relativePath = input.StartsWith(basePath) ? input.Remove(0, basePath.Length) : input;
            var hash = themeEngine.GetAssetHash(relativePath);
            return input.Contains('?') ? $"{input}&v={hash}" : $"{input}?v={hash}";
        }

        private static string BuildOnClickLink(string title, string onclickFormat, params object[] onclickArgs)
        {
            var onClick = string.Format(CultureInfo.InvariantCulture, onclickFormat, onclickArgs);

            var result = string.Format(CultureInfo.InvariantCulture, "<a href=\"#\" onclick=\"{0}\">{1}</a>",
                HttpUtility.HtmlAttributeEncode(onClick),
                HttpUtility.HtmlEncode(title));

            return result;
        }

        private static string BuildHtmlLink(string id, string virtualUrl, string title)
        {
            var href = BuildAbsoluteUrl(virtualUrl);

            var result = string.Format(CultureInfo.InvariantCulture, "<a href=\"{0}\" id=\"{1}\">{2}</a>",
                HttpUtility.HtmlAttributeEncode(href),
                HttpUtility.HtmlAttributeEncode(id),
                HttpUtility.HtmlEncode(title));

            return result;
        }

        private enum TagAction
        {
            Add,
            Remove,
            Replace
        }

        private static string BuildTagLink(TagAction action, object tagObject, object input)
        {
            var href = string.Empty;
            var title = string.Empty;
            var label = input.ToString();

            if (tagObject == null)
            {
                title = "Remove all tags";
                href = GetCurrentUrlWithTags(TagAction.Replace, null, null);
            }
            else
            {
                var tag = tagObject as shopifyModel.Tag;
                if (tag != null)
                {
                    href = GetCurrentUrlWithTags(action, tag.GroupName, tag.Value);
                    title = BuildTagActionTitle(action, label);

                    if (tag.Count > 0)
                    {
                        label = $"{label} ({tag.Count})";
                    }
                }
                else
                {
                    // TODO: Parse tag string
                }
            }

            var result = string.Format(CultureInfo.InvariantCulture, "<a href=\"{0}\" title=\"{1}\">{2}</a>",
                HttpUtility.HtmlAttributeEncode(href),
                HttpUtility.HtmlAttributeEncode(title),
                HttpUtility.HtmlEncode(label));

            return result;
        }

        private static string BuildTagActionTitle(TagAction action, string tagLabel)
        {
            switch (action)
            {
                case TagAction.Remove:
                    return $"Remove tag '{tagLabel}'";
                default:
                    return $"Show products matching tag '{tagLabel}'";
            }
        }

        private static string GetCurrentUrlWithTags(TagAction action, string groupName, string value)
        {
            var themeEngine = (ShopifyLiquidThemeEngine)Template.FileSystem;
            var workContext = themeEngine.WorkContext;

            var terms = workContext.CurrentProductSearchCriteria.Terms
                .Select(t => new Term { Name = t.Name, Value = t.Value })
                .ToList();

            switch (action)
            {
                case TagAction.Add:
                    terms.Add(new Term { Name = groupName, Value = value });
                    break;
                case TagAction.Remove:
                    terms.RemoveAll(t =>
                        string.Equals(t.Name, groupName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(t.Value, value, StringComparison.OrdinalIgnoreCase));
                    break;
                case TagAction.Replace:
                    terms.Clear();

                    if (!string.IsNullOrEmpty(groupName))
                    {
                        terms.Add(new Term { Name = groupName, Value = value });
                    }
                    break;
            }

            var termsString = terms.Any() ? string.Join(";", terms.ToStrings()) : null;
            var url = workContext.RequestUrl.SetQueryParameter("terms", termsString);

            return url.AbsoluteUri;
        }


        private static string BuildAbsoluteUrl(string virtualUrl)
        {
            var themeEngine = (ShopifyLiquidThemeEngine)Template.FileSystem;
            var workContext = themeEngine.WorkContext;
            var result = themeEngine.UrlBuilder.ToAppAbsolute(virtualUrl, workContext.CurrentStore, workContext.CurrentLanguage);
            return result;
        }
    }
}
