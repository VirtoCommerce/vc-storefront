using System.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Tools;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class StaticContentItemBuilder
    {
        private class UrlsParser : IContentItemParser
        {
            public bool Suit(ContentItem item)
            {
                return true;
            }

            public void Parse(string path, string content, ContentItem item)
            {
                if (string.IsNullOrEmpty(item.Permalink))
                {
                    item.Permalink = ":folder/:categories/:title";
                }

                // Transform permalink template to url
                item.Url = GetContentItemUrl(item, item.Permalink);
                // Transform aliases permalink templates to urls
                item.AliasesUrls = item.Aliases.Select(x => GetContentItemUrl(item, x)).ToList();
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
}
