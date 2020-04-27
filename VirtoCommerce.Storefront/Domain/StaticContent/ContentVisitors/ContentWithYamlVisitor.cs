using System.Linq;
using Markdig;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal class ContentWithYamlVisitor : IContentItemVisitor
    {
        private readonly MarkdownPipeline _markdownPipeline;

        public ContentWithYamlVisitor(MarkdownPipeline markdownPipeline)
        {
            _markdownPipeline = markdownPipeline;
        }

        public bool Suit(ContentItem item)
        {
            return StaticContentItemBuilder.extensions.Any(item.FileName.EndsWith);
        }

        public ContentItem Parse(string path, string content, ContentItem item)
        {
            var result = RemoveYamlHeader(content);
            item.Content = Markdown.ToHtml(result, _markdownPipeline);
            return item;
        }

        private static string RemoveYamlHeader(string text)
        {
            var result = text;
            var headerMatches = StaticContentItemBuilder.headerRegExp.Matches(text);

            if (headerMatches.Count > 0)
            {
                result = text.Replace(headerMatches[0].Groups[0].Value, "").Trim();
            }

            return result;
        }
    }
}
