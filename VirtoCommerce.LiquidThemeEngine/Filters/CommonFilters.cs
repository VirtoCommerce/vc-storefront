using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using DotLiquid.ViewEngine.Extensions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PagedList.Core;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;
using VirtoCommerce.LiquidThemeEngine.Extensions;
using VirtoCommerce.LiquidThemeEngine.JsonConverters;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    public static partial class CommonFilters
    {
        private static readonly string[] _poweredLinks = {
            "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">.NET ecommerce platform</a> by Virto",
            "<a href=\"http://virtocommerce.com/shopping-cart\" rel=\"nofollow\" target=\"_blank\">Shopping Cart</a> by Virto",
            "<a href=\"http://virtocommerce.com/shopping-cart\" rel=\"nofollow\" target=\"_blank\">.NET Shopping Cart</a> by Virto",
            "<a href=\"http://virtocommerce.com/shopping-cart\" rel=\"nofollow\" target=\"_blank\">ASP.NET Shopping Cart</a> by Virto",
            "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">.NET ecommerce</a> by Virto",
            "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">.NET ecommerce framework</a> by Virto",
            "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">ASP.NET ecommerce</a> by Virto Commerce",
            "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">ASP.NET ecommerce platform</a> by Virto",
            "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">ASP.NET ecommerce framework</a> by Virto",
            "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">Enterprise ecommerce</a> by Virto",
            "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">Enterprise ecommerce platform</a> by Virto",
        };

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            },
            Converters = new List<JsonConverter>
            {
                new MutablePagedListAsArrayJsonConverter(),
            },
        };

        public static object Default(object input, object value)
        {
            return input ?? value;
        }

        public static string Json(object input)
        {
            var serializedString = input != null ? JsonConvert.SerializeObject(input, _jsonSerializerSettings) : null;
            return serializedString;
        }

        public static object ParseJson(string input)
        {
            var result = input != null ? JsonConvert.DeserializeObject(input, _jsonSerializerSettings) : null;
            return result;
        }

        public static string PoweredBy(string signature)
        {
            var hashCode = (uint)signature.GetHashCode();
            return _poweredLinks[hashCode % _poweredLinks.Length];
        }

        public static string Render(TemplateContext context, string input)
        {
            if (input == null)
            {
                return null;
            }
            var themeEngine = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var result = themeEngine.RenderTemplateAsync(input, null, context.CurrentGlobal).GetAwaiter().GetResult();
            return result;
        }

        public static string Antiforgery(TemplateContext context)
        {
            var themeEngine = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var httpContext = themeEngine.HttpContext;
            var antiforgery = httpContext.RequestServices.GetService<IAntiforgery>();
            var htmlContent = antiforgery.GetHtml(httpContext);
            var writer = new StringWriter();
            htmlContent.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static string Layout(TemplateContext context, string layout)
        {
            if (!string.IsNullOrEmpty(layout))
            {
                var layoutSetter = (Action<string>)context.GetValue(new ScriptVariableGlobal("layout_setter"));
                layoutSetter(layout);
            }
            return null;
        }

        public static Paginate Paginate(TemplateContext context, object source, int pageSize = 20, string filterJson = null)
        {
            var pagedList = source as IPagedList;
            var requestUrl = context.GetValue(new ScriptVariableGlobal("request_url")) as Uri;
            var pageNumber = context.GetValue(new ScriptVariableGlobal("page_number"))?.ToString().SafeParseInt(1) ?? 1;
            var effectivePageSize = context.GetValue(new ScriptVariableGlobal("page_size"))?.ToString().SafeParseInt(pageSize) ?? pageSize;
            var @params = new NameValueCollection();

            if (!string.IsNullOrEmpty(filterJson))
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(filterJson);
                foreach (var pair in values)
                {
                    @params.Add(pair.Key, pair.Value);
                }
            }

            switch (source)
            {
                case IMutablePagedList mutablePagedList:
                    mutablePagedList.Slice(pageNumber, effectivePageSize, mutablePagedList.SortInfos, @params);
                    pagedList = mutablePagedList;
                    break;
                case ScriptObject scriptObject when scriptObject.Keys.Contains("total_count"):
                    pagedList = new StaticPagedList<object>(Array.Empty<object>(), pageNumber, effectivePageSize, scriptObject["total_count"].ToString().SafeParseInt(0));
                    break;
                case ICollection collection:
                    pagedList = new PagedList<object>(collection.OfType<object>().AsQueryable(), pageNumber, effectivePageSize);
                    break;
            }

            if (pagedList == null)
            {
                return null;
            }

            var result = new Paginate(pagedList);

            for (var i = 1; i <= pagedList.PageCount; i++)
            {
                var page = i > 1 ? i.ToString() : null;

                var part = new Part
                {
                    IsLink = i != pagedList.PageNumber,
                    Title = i.ToString(),
                    Url = requestUrl != null ? requestUrl.SetQueryParameter("page", page).ToString() : i.ToString()
                };

                result.Parts.Add(part);
            }

            return result;
        }

        public static string Setting(TemplateContext context, string key)
        {
            var themeEngine = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var settings = themeEngine.GetSettings();

            settings.TryGetValue(key, out var result);

            return result?.ToString();
        }
    }
}
