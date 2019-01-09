using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.Antiforgery;
using Newtonsoft.Json;
using PagedList.Core;
using Scriban;
using Scriban.Syntax;
using VirtoCommerce.LiquidThemeEngine.Extensions;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Text.Encodings.Web;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    public class CommonFilters
    {
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

            var serializedString = JsonConvert.SerializeObject(
               input,
               new JsonSerializerSettings()
               {
                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                   //TODO:
                   //ContractResolver = new RubyContractResolver(),
               });

            return serializedString;
        }

        public static string Render(TemplateContext context, string input)
        {
            if (input == null)
            {
                return null;
            }
            var themeEngine = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var result = themeEngine.RenderTemplate(input, null, context.CurrentGlobal);
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
            Uri.TryCreate(context.GetValue(new ScriptVariableGlobal("request_url")) as string, UriKind.RelativeOrAbsolute, out var requestUrl);
            var pageNumber = (int)context.GetValue(new ScriptVariableGlobal("current_page"));
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

