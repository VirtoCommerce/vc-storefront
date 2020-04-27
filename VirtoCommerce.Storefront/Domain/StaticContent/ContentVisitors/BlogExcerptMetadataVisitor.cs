using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{

    internal class BlogExcerptMetadataVisitor : IContentItemVisitor
    {
        private static string _excerptToken = "<!--excerpt-->";

        public bool Suit(ContentItem item)
        {
            return item is BlogArticle && StaticContentItemBuilder.extensions.Any(item.FileName.EndsWith);
        }

        public ContentItem Parse(string path, string content, ContentItem item)
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
            return item;
        }
    }
}
