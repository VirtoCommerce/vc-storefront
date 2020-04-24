using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class StaticContentItemBuilder
    {
        private class BlogExcerptMetadataParser : IContentItemParser
        {
            private static string _excerptToken = "<!--excerpt-->";

            public bool Suit(ContentItem item)
            {
                return item is BlogArticle && _extensions.Any(item.FileName.EndsWith);
            }

            public void Parse(string path, string content, ContentItem item)
            {
                var post = (BlogArticle)item;
                if (content.Contains(_excerptToken))
                {
                    var parts = content.Split(new[] { _excerptToken }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        post.Excerpt = parts[0];
                        post.Content = content.Replace(_excerptToken, string.Empty);
                    }
                }

            }
        }
    }
}
