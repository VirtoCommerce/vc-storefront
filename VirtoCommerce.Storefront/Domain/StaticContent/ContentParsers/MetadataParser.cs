using System;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class StaticContentItemBuilder
    {
        private class MetadataParser : IContentItemParser
        {
            public bool Suit(ContentItem item)
            {
                return true;
            }

            public void Parse(string path, string content, ContentItem item)
            {
                foreach (var setting in item.MetaInfo)
                {
                    var settingValue = setting.Value.FirstOrDefault();
                    switch (setting.Key.ToLower())
                    {
                        case "permalink":
                            item.Permalink = settingValue;
                            break;

                        case "aliases":
                            item.Aliases = setting.Value.ToList();
                            break;

                        case "title":
                            item.Title = settingValue;
                            break;

                        case "author":
                            item.Author = settingValue;
                            break;

                        case "published":
                            item.IsPublished = !bool.TryParse(settingValue, out var isPublished) || isPublished;
                            break;

                        case "date":
                            item.PublishedDate = item.CreatedDate = DateTime.TryParse(settingValue, out var date) ? date : new DateTime();
                            break;
                        case "tags":
                            item.Tags = setting.Value.OrderBy(t => t).Select(t => t.Handelize()).ToList();
                            break;

                        case "categories":
                            item.Categories = setting.Value?.Select(x => x.Handelize()).ToList();
                            break;

                        case "category":
                            item.Category = settingValue?.Handelize();
                            break;

                        case "layout":
                            item.Layout = settingValue;
                            break;

                        case "priority":
                            item.Priority = int.TryParse(settingValue, out var priority) ? priority : 0;
                            break;

                        case "description":
                            item.Description = settingValue;
                            break;

                        case "authorize":
                            if (bool.TryParse(settingValue, out var isAuthorize))
                            {
                                item.Authorize = isAuthorize;
                            }
                            break;
                    }
                }
                if (item.Title == null)
                {
                    item.Title = item.Name;
                }

                switch (item)
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
    }
}
