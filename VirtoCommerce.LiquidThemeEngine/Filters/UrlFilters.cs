using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Scriban;
using VirtoCommerce.LiquidThemeEngine.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Order;
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
        /// Generates a link to the customer login page.
        /// {{ 'Log in' | customer_login_link }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CustomerLoginLink(TemplateContext context, string input)
        {
            return BuildHtmlLink(context, "customer_login_link", "~/account/login", input);
        }
        public static string CustomerRegisterLink(TemplateContext context, string input)
        {
            return BuildHtmlLink(context, "customer_register_link", "~/account/register", input);
        }

        public static string CustomerLogoutLink(TemplateContext context, string input)
        {
            return BuildHtmlLink(context, "customer_logout_link", "~/account/logout", input);
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
            {
                return null;
            }

            var retVal = input.ToString();

            if (input is Product product)
            {
                retVal = product.PrimaryImage?.Url;
            }
            if (input is Image image)
            {
                retVal = image.Url;
            }
            if (input is Category collection)
            {
                retVal = collection.PrimaryImage?.Url;
            }
            if (input is LineItem lineItem)
            {
                retVal = lineItem.ImageUrl;
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
        [Obsolete("Left for backward compatibility with Shopify syntax. Need to use AddTermUrl instead")]
        public static string LinkToTag(TemplateContext context, object input, object tag)
        {
            return BuildTagLink(context, TagAction.Replace, tag, input);
        }

        /// <summary>
        /// Creates a link to all products in a collection that have a given tag as well as any tags that have been already selected.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        [Obsolete("Left for backward compatibility with Shopify syntax. Need to use AddTermUrl instead")]
        public static string LinkToAddTag(TemplateContext context, object input, object tag)
        {
            return BuildTagLink(context, TagAction.Add, tag, input);
        }

        /// <summary>
        /// Generates a link to all products in a collection that have all the previous tags that might have been added already, excluding the given tag.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        [Obsolete("Left for backward compatibility with Shopify syntax. Need to use RemoveTermUrl instead")]
        public static string LinkToRemoveTag(TemplateContext context, object input, object tag)
        {
            return BuildTagLink(context, TagAction.Remove, tag, input);
        }


        public static string SortLinkUrlLinkToRemoveTag(TemplateContext context, object input, object tag)
        {
            return BuildTagLink(context, TagAction.Remove, tag, input);
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


        public static string TermsToString(TemplateContext context, IList<Term> terms)
        {
            return string.Join(";", terms.ToStrings() ?? new List<string> { });          
        }

        /// <summary>
        /// Generates an relative url with query string that contains serialized ProductSearchCriteria as parameters
        /// and adds to it sort_by expression with given sortBy value
        /// <param name="context"></param>
        /// <param name="sortBy">sort expression e.g popularity:desc </param>
        /// <returns>example: /collection?sort_by=popularity:desc</returns>
        public static string SortByUrl(TemplateContext context, string sortBy)
        {
            return BuildUriWithSearchQueryString(context, criteria =>
            {
                criteria.SortBy = sortBy;
            }).PathAndQuery;
        }

        /// <summary>
        ///  Generates an relative url with query string that contains serialized ProductSearchCriteria as parameters
        ///  and adds to it page_size parameter with  given  value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pageSize">a new page size</param>
        /// <returns>example: /collection?page_szie=20</returns>
        public static string PageSizeUrl(TemplateContext context, int pageSize)
        {
            return BuildUriWithSearchQueryString(context, criteria =>
            {
                criteria.PageSize = pageSize;
            }).PathAndQuery;
        }


        /// <summary>
        ///  Generates an relative url with query string that contains serialized ProductSearchCriteria as parameters
        ///  and adds to it q parameter with  given keyword value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="keyword">search keyword</param>
        /// <returns>example: /collection?q=foo</returns>
        public static string AddSearchByKeywordUrl (TemplateContext context, string keyword)
        {
            return BuildUriWithSearchQueryString(context, criteria =>
            {
                criteria.Keyword = keyword;
                //Need to reset page number every time when criteria changed
                criteria.PageNumber = 1;
            }).PathAndQuery;
        }

        /// <summary>
        ///  Generates an relative url with query string that contains serialized ProductSearchCriteria as parameters
        ///  and remove  q parameter from  resulting query string
        /// </summary>
        /// <param name="context"></param>
        /// <param name="keyword">search keyword</param>
        /// <returns>example: /collection</returns>
        public static string RemoveSearchByKeywordUrl(TemplateContext context)
        {
            return BuildUriWithSearchQueryString(context, criteria =>
            {
                criteria.Keyword = null;
                //Need to reset page number every time when criteria changed
                criteria.PageNumber = 1;
            }).PathAndQuery;
        }

        /// <summary>
        ///  Generates an relative url with query string that contains serialized ProductSearchCriteria as parameters
        ///  and add a new given aggregation item value  to  terms parameter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="aggregationItem">aggregation item</param>
        /// <returns>example: /collection?terms=color:Red</returns>
        public static string AddTermUrl(TemplateContext context, AggregationItem aggregationItem)
        {
            Uri result = null;
            result = BuildUriWithSearchQueryString(context, criteria =>
            {
                var term = new Term { Name = aggregationItem.Group.Field, Value = aggregationItem.Value.ToString() };
                criteria.Terms.Add(term);
                //Need to reset page number every time when criteria changed
                criteria.PageNumber = 1;
            });

            return result?.PathAndQuery;
        }

        /// <summary>
        ///  Generates an relative url with query string that contains serialized ProductSearchCriteria as parameters
        ///  and remove a given aggregation item value  from  terms parameter
        /// </summary>
        /// <param name="context"></param>
        /// <param name="aggregationItem">aggregation item</param>
        /// <returns>example: /collection</returns>
        public static string RemoveTermUrl(TemplateContext context, AggregationItem aggregationItem)
        {
            Uri result = null;

            result = BuildUriWithSearchQueryString(context, criteria =>
            {
                var term = new Term { Name = aggregationItem.Group.Field, Value = aggregationItem.Value.ToString() };
                criteria.Terms.Remove(term);
                //Need to reset page number every time when criteria changed
                criteria.PageNumber = 1;
            });
            return result?.PathAndQuery;
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

        public static string Within(TemplateContext context, string input, object collection)
        {
            return BuildAbsoluteUrl(context, input);
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

        private static string BuildOnClickLink(string title, string onclickFormat, params object[] onclickArgs)
        {
            var onClick = string.Format(CultureInfo.InvariantCulture, onclickFormat, onclickArgs);

            var result = string.Format(CultureInfo.InvariantCulture, "<a href=\"#\" onclick=\"{0}\">{1}</a>",
                HttpUtility.HtmlAttributeEncode(onClick),
                HttpUtility.HtmlEncode(title));

            return result;
        }

        private static string BuildHtmlLink(TemplateContext context, string id, string virtualUrl, string title)
        {
            var href = BuildAbsoluteUrl(context, virtualUrl);

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

        private static string BuildTagLink(TemplateContext context, TagAction action, object tagObject, object input)
        {
            var href = string.Empty;
            var title = string.Empty;
            var label = input.ToString();

            if (tagObject == null)
            {
                title = "Remove all tags";
                href = GetCurrentUrlWithTags(context, TagAction.Replace, null, null);
            }
            else
            {
                if (tagObject is AggregationItem aggregationItem)
                {
                    href = GetCurrentUrlWithTags(context, action, aggregationItem.GroupLabel, aggregationItem.Value.ToString());
                    title = BuildTagActionTitle(action, label);

                    if (aggregationItem.Count > 0)
                    {
                        label = $"{label} ({aggregationItem.Count})";
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

        private static string GetCurrentUrlWithTags(TemplateContext context, TagAction action, string groupName, string value)
        {
            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var workContext = themeAdaptor.WorkContext;

            var criteria = workContext.CurrentProductSearchCriteria.Clone() as ProductSearchCriteria;
            var term = new Term { Name = groupName, Value = value };
            switch (action)
            {
                case TagAction.Add:
                    criteria.Terms.Add(term);
                    break;
                case TagAction.Remove:
                    criteria.Terms.Remove(term);
                    break;
                case TagAction.Replace:
                    criteria.Terms.Clear();
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        criteria.Terms.Add(term);
                    }
                    break;
            }
            var result = workContext.RequestUrl.SetQueryParameters(criteria);
            return result.PathAndQuery;
        }


        private static Uri BuildUriWithSearchQueryString(TemplateContext context, Action<ProductSearchCriteria> criteriaAction)
        {
            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var workContext = themeAdaptor.WorkContext;

            var criteria = workContext.CurrentProductSearchCriteria.Clone() as ProductSearchCriteria;
            criteriaAction(criteria);
            return workContext.RequestUrl.SetQueryParameters(criteria);
        }

        private static string BuildAbsoluteUrl(TemplateContext context, string virtualUrl)
        {
            var themeEngine = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var workContext = themeEngine.WorkContext;
            var result = themeEngine.UrlBuilder.ToAppAbsolute(virtualUrl, workContext.CurrentStore, workContext.CurrentLanguage);
            return result;
        }
    }
}
