using DotLiquid;
using DotLiquid.Exceptions;
using DotLiquid.Util;
using Newtonsoft.Json;
using PagedList.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VirtoCommerce.LiquidThemeEngine.Extensions;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Tags
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/tags/theme-tags#paginate
    /// Splitting products, blog articles, and search results across multiple pages is a necessary component of theme design as you are limited to 50 results per page in any for loop.
    /// The paginate tag works in conjunction with the for tag to split content into numerous pages.It must wrap a for tag block that loops through an array, as shown in the example below:
    /// </summary>
    public class PaginateTag : Block
    {
        private static readonly Regex _syntax = R.B(R.Q(@"({0})\s*by\s*({0}+)?"), Liquid.QuotedFragment);
        private static readonly Regex _paramsSyntax = new Regex(@"({[\w\:"", ]+})");
        private string _collectionName;
        private string _paginateBy;
        private NameValueCollection _params;



        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            var match = _syntax.Match(markup);

            if (match.Success)
            {
                _collectionName = match.Groups[1].Value;
                _paginateBy = match.Groups[2].Value;
                var paramsMatch = _paramsSyntax.Match(markup);
                if (paramsMatch.Success)
                {
                    var json = paramsMatch.Groups[0].Value;
                    var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    _params = new NameValueCollection();
                    foreach (var pair in values)
                    {
                        _params.Add(pair.Key, pair.Value);
                    }
                }
            }
            else
            {
                throw new SyntaxException("PaginateSyntaxException");
            }

            base.Initialize(tagName, markup, tokens);
        }

        public override void Render(Context context, TextWriter result)
        {
            var mutablePagedList = context[_collectionName] as IMutablePagedList;
            var collection = context[_collectionName] as ICollection;
            var pagedList = context[_collectionName] as IPagedList;
            Uri requestUrl;
            Uri.TryCreate(context["request_url"] as string, UriKind.RelativeOrAbsolute, out requestUrl);
            var pageNumber = (int)context["current_page"];
            var globalPageSize = (int)context["page_size"];
            var localPageSize = GetIntegerValue(_paginateBy, context, 20);

            if (mutablePagedList != null)
            {
                mutablePagedList.Slice(pageNumber, globalPageSize > 0 ? globalPageSize : localPageSize, mutablePagedList.SortInfos, _params);
                pagedList = mutablePagedList;
            }
            else if (collection != null)
            {
                pagedList = new PagedList<Drop>(collection.OfType<Drop>().AsQueryable(), pageNumber, localPageSize);
                //TODO: Need find way to replace ICollection instance in liquid context to paged instance
                //var hash = context.Environments.FirstOrDefault(s => s.ContainsKey(_collectionName));
                //hash[_collectionName] = pagedList;
            }

            if (pagedList != null)
            {
                var paginate = new Paginate(pagedList);

                for (var i = 1; i <= pagedList.PageCount; i++)
                {
                    var part = new Part
                    {
                        IsLink = i != pagedList.PageNumber,
                        Title = i.ToString(),
                        Url = requestUrl != null ? requestUrl.SetQueryParameter("page", i > 1 ? i.ToString() : null).ToString() : i.ToString()
                    };

                    paginate.Parts.Add(part);
                }

                context["paginate"] = paginate;
                RenderAll(NodeList, context, result);
            }
        }

        private static int GetIntegerValue(string paginateBy, Context context, int defaultValue)
        {
            int? result = null;

            int pageSize;
            if (int.TryParse(paginateBy, out pageSize))
            {
                result = pageSize;
            }

            if (result == null && context.HasKey(paginateBy))
            {
                result = Convert.ToInt32(context[paginateBy]);
            }

            return result ?? defaultValue;
        }
    }
}
