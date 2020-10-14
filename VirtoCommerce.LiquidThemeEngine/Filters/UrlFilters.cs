using System;
using System.Linq;
using Scriban;
using VirtoCommerce.LiquidThemeEngine.Extensions;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;
using storefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/filters/url-filters
    /// </summary>
    public static partial class UrlFilters
    {
        public static string SizeImageLink(string input, string size)
        {
            if (input != null)
            {
                input = input.AddSuffixToFileUrl("_" + size.TrimStart('_'));
            }
            return input;
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
            {
                return null;
            }

            var retVal = input.ToString();
           
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
        /// Returns the URL of a file in the "assets" folder of a theme.
        /// {{ 'shop.css' | asset_url }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string AssetUrl(TemplateContext context, string input)
        {
            string retVal = null;
            if (input != null)
            {
                var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
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
        public static string StaticAssetUrl(TemplateContext context, string input)
        {
            string retVal = null;
            if (input != null)
            {
                var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
                retVal = themeAdaptor.GetAssetAbsoluteUrl("static/" + input.TrimStart('/'));
            }
            return retVal;
        }

        /// <summary>
        /// Returns the URL of a file.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FileUrl(TemplateContext context, string input)
        {
            return AssetUrl(context, input);
        }

        /// <summary>
        /// Method for switching between multiple stores
        /// </summary>
        /// <param name="input"></param>
        /// <param name="storeId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static string StoreAbsoluteUrl(TemplateContext context, string input, string storeId = null, string languageCode = null)
        {
            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            Store store = null;
            if (!string.IsNullOrEmpty(storeId))
            {
                store = themeAdaptor.WorkContext.AllStores.FirstOrDefault(x => string.Equals(x.Id, storeId, StringComparison.InvariantCultureIgnoreCase));
            }
            store = store ?? themeAdaptor.WorkContext.CurrentStore;

            var retVal = AbsoluteUrl(context, input, storeId, languageCode);

            var isHttps = themeAdaptor.WorkContext.RequestUrl.Scheme == Uri.UriSchemeHttps;
            //If store has defined url need redirect to it
            if (isHttps)
            {
                retVal = string.IsNullOrEmpty(store.SecureUrl) ? retVal : store.SecureUrl;
            }
            else
            {
                retVal = string.IsNullOrEmpty(store.Url) ? retVal : store.Url;
            }
            return retVal;
        }

        public static string FullUrl(TemplateContext context, string input, string storeId = null, string languageCode = null)
        {
            var absoluteUrl = AbsoluteUrl(context, input, storeId, languageCode);

            var themeEngine = (ShopifyLiquidThemeEngine)context.TemplateLoader;
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
        public static string AbsoluteUrl(TemplateContext context, string input, string storeId = null, string languageCode = null)
        {
            if (input == null)
            {
                return string.Empty;
            }

            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
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

        /// <summary>
        /// Appends hash of file content as file version to invalidate browser cache when file changed.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string AppendVersion(TemplateContext context, string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var basePath = themeAdaptor.GetAssetAbsoluteUrl("");
            var relativePath = input.StartsWith(basePath) ? input.Remove(0, basePath.Length) : input;
            var hash = themeAdaptor.GetAssetHash(relativePath);
            return input.Contains('?') ? $"{input}&v={hash}" : $"{input}?v={hash}";
        }


        /// <summary>
        ///  Generates an relative url with query string that contains serialized ProductSearchCriteria as parameters
        ///  and add a new given aggregation item value  to  terms parameter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="aggregationItem">aggregation item</param>
        /// <returns>example: /collection?terms=color:Red</returns>
        public static string AddTermUrl(TemplateContext context, string facetName, string term)
        {
            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var result = themeAdaptor.WorkContext.RequestUrl.SetQueryParameter("filter", $"{facetName}:{term}");         
            return result?.PathAndQuery;
        }

        /// <summary>
        ///  Generates an relative url with query string that contains serialized ProductSearchCriteria as parameters
        ///  and remove a given aggregation item value  from  terms parameter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="aggregationItem">aggregation item</param>
        /// <returns>example: /collection</returns>
        public static string RemoveTermUrl(TemplateContext context, string facetName, string term)
        {
            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var result = themeAdaptor.WorkContext.RequestUrl.SetQueryParameter("filter", null);
            return result?.PathAndQuery;
        }
             
    }
}
