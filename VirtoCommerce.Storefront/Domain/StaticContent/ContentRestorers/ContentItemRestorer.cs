using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Tools;

namespace VirtoCommerce.Storefront.Domain
{
    public abstract class ContentItemRestorer : IContentItemRestorer
    {
        private static readonly string[] _extensions = new[] { ".md", ".liquid", ".html" };

        public void FulfillContent(IContentItemReader reader, ContentItem contentItem)
        {
            ApplyMetadata(reader.ReadMetadata(), contentItem);
            ApplyContent(reader.ReadContent(), contentItem);
        }

        protected virtual void ApplyMetadata(Dictionary<string, IEnumerable<string>> metadata, ContentItem contentItem)
        {
            contentItem.MetaInfo = metadata;
            SetIfExists(contentItem, "template", () => contentItem.Template);
            SetIfExists(contentItem, "permalink", () => contentItem.Permalink);
            SetIfExists(contentItem, "aliases", () => contentItem.Aliases);
            SetIfExists(contentItem, "title", () => contentItem.Title);
            SetIfExists(contentItem, "author", () => contentItem.Author);
            SetIfExists(contentItem, "published", () => contentItem.IsPublished);
            SetIfExists(contentItem, "layout", () => contentItem.Layout);
            SetIfExists(contentItem, "description", () => contentItem.Description);
            SetIfExists(contentItem, "authorize", () => contentItem.Authorize);
            SetIfExists(contentItem, "date", () => contentItem.PublishedDate);
            SetIfExists(contentItem, "priority", () => contentItem.Priority);
            SetIfExists(contentItem, "tags", () => contentItem.Tags);
            SetIfExists(contentItem, "categories", () => contentItem.Categories);
            SetIfExists(contentItem, "category", () => contentItem.Category);

            contentItem.CreatedDate = contentItem.PublishedDate ?? DateTime.Now;
            contentItem.Tags = contentItem.Tags.OrderBy(x => x).Select(x => x.Handelize()).ToList();
            contentItem.Categories = contentItem.Categories.Select(x => x.Handelize()).ToList();
            contentItem.Category = contentItem.Category.Handelize();

            if (contentItem.Name.IsNullOrEmpty())
            {
                SetIfExists(contentItem, "contentItemName", () => contentItem.Name);
            }
            if (contentItem.Title.IsNullOrEmpty())
            {
                contentItem.Title = contentItem.Name;
            }
            SetLanguage(metadata, contentItem);
            SetTemplate(contentItem);
            SetUrls(contentItem);
        }

        protected void SetIfExists(ContentItem item, string key, Expression<Func<object>> expr)
        {
            if (item.MetaInfo.ContainsKey(key))
            {
                var propInfo = (PropertyInfo)(expr.Body.NodeType == ExpressionType.Convert
                    ? (MemberExpression)((UnaryExpression)expr.Body).Operand
                    : (MemberExpression)expr.Body).Member;

                var value = item.MetaInfo[key].FirstOrDefault();
                if (propInfo.PropertyType == typeof(string))
                {
                    propInfo.SetValue(item, value);
                }
                else if (propInfo.PropertyType == typeof(bool))
                {
                    bool.TryParse(value, out var result);
                    propInfo.SetValue(item, result);
                }
                else if (propInfo.PropertyType == typeof(DateTime) || propInfo.PropertyType == typeof(DateTime?))
                {
                    propInfo.SetValue(item, DateTime.TryParse(value, out var date) ? date : new DateTime());
                }
                else if (propInfo.PropertyType == typeof(int))
                {
                    propInfo.SetValue(item, int.TryParse(value, out var result) ? result : default);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType))
                {
                    propInfo.SetValue(item, item.MetaInfo[key].ToList());
                }
            }

        }

        protected virtual void ApplyContent(string content, ContentItem contentItem)
        {
            contentItem.Content = content;
        }

        protected void SetLanguage(Dictionary<string, IEnumerable<string>> metadata, ContentItem contentItem)
        {
            if (metadata.ContainsKey("language"))
            {
                var value = metadata["language"].FirstOrDefault();
                if (!value.IsNullOrEmpty() && contentItem.Language == null)
                {
                    try
                    {
                        contentItem.Language = new Language(value);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            if (contentItem.Language == null)
            {
                contentItem.Language = Language.InvariantLanguage;
            }
        }

        protected void SetTemplate(ContentItem contentItem)
        {
            if (contentItem.Template.IsNullOrEmpty())
            {
                switch (contentItem)
                {
                    case ContentPage page when page.FileName.EndsWith(".page"):
                        page.Template = "json-page";
                        break;
                    case ContentPage page when _extensions.Any(page.FileName.EndsWith):
                        page.Template = "page";
                        break;
                    case BlogArticle page when page.FileName.EndsWith(".page"):
                        page.Template = "json-article";
                        break;
                    case BlogArticle page when _extensions.Any(page.FileName.EndsWith):
                        page.Template = "article";
                        break;
                }
            }
        }

        protected void SetUrls(ContentItem contentItem)
        {
            if (string.IsNullOrEmpty(contentItem.Permalink))
            {
                contentItem.Permalink = ":folder/:categories/:title";
            }

            // Transform permalink template to url
            contentItem.Url = GetContentItemUrl(contentItem, contentItem.Permalink);
            // Transform aliases permalink templates to urls
            contentItem.AliasesUrls = contentItem.Aliases.Select(x => GetContentItemUrl(contentItem, x)).ToList();
        }

        private static string GetContentItemUrl(ContentItem item, string permalink)
        {
            return new FrontMatterPermalink
            {
                UrlTemplate = permalink,
                Categories = item.Categories,
                Date = item.CreatedDate,
                FilePath = item.StoragePath
            }.ToUrl();
        }
    }
}
