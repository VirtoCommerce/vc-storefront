using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PagedList.Core;
using Scriban;
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

        public static object Default(object input, object value)
        {
            return input ?? value;
        }

        public static string Json(object input)
        {
            if (input == null)
            {
                return null;
            }

            var jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
                //TODO: 
                //ContractResolver = new RubyContractResolver(),
            };
            //TODO: duplicated code of custom json convertor MutablePagedListAsArrayJsonConverter. 
            jsonSettings.Converters.Add(new MutablePagedListAsArrayJsonConverter());
            var serializedString = JsonConvert.SerializeObject(input, jsonSettings);
            return serializedString;
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
            Paginate result = null;
            var collection = source as ICollection;
            var pagedList = source as IPagedList;
            var requestUrl = context.GetValue(new ScriptVariableGlobal("request_url")) as Uri;
            var pageNumber = (int)(context.GetValue(new ScriptVariableGlobal("page_number")) ?? 1);
            var @params = new NameValueCollection();
            if (!string.IsNullOrEmpty(filterJson))
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(filterJson);
                foreach (var pair in values)
                {
                    @params.Add(pair.Key, pair.Value);
                }
            }
            if (source is IMutablePagedList mutablePagedList)
            {
                mutablePagedList.Slice(pageNumber, pageSize, mutablePagedList.SortInfos, @params);
                pagedList = mutablePagedList;
            }
            else if (collection != null)
            {
                pagedList = new PagedList<object>(collection.OfType<object>().AsQueryable(), pageNumber, pageSize);
                //TODO: Need find way to replace ICollection instance in liquid context to paged instance
                //var hash = context.Environments.FirstOrDefault(s => s.ContainsKey(_collectionName));
                //hash[_collectionName] = pagedList;
            }

            if (pagedList != null)
            {
                result = new Paginate(pagedList);

                for (var i = 1; i <= pagedList.PageCount; i++)
                {
                    var part = new Part
                    {
                        IsLink = i != pagedList.PageNumber,
                        Title = i.ToString(),
                        Url = requestUrl != null ? requestUrl.SetQueryParameter("page", i > 1 ? i.ToString() : null).ToString() : i.ToString()
                    };

                    result.Parts.Add(part);
                }
            }
            return result;
        }
    }

}

