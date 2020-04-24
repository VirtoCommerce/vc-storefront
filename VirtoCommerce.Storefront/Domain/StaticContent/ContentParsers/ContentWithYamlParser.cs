using System.Linq;
using Markdig;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class StaticContentItemBuilder
    {
        private class ContentWithYamlParser : IContentItemParser
        {
            private readonly MarkdownPipeline _markdownPipeline;

            public ContentWithYamlParser(MarkdownPipeline markdownPipeline)
            {
                _markdownPipeline = markdownPipeline;
            }

            public bool Suit(ContentItem item)
            {
                return _extensions.Any(item.FileName.EndsWith);
            }

            public void Parse(string path, string content, ContentItem item)
            {
                var result = RemoveYamlHeader(content);
                item.Content = Markdown.ToHtml(result, _markdownPipeline);
            }

            private static string RemoveYamlHeader(string text)
            {
                var result = text;
                var headerMatches = _headerRegExp.Matches(text);

                if (headerMatches.Count > 0)
                {
                    result = text.Replace(headerMatches[0].Groups[0].Value, "").Trim();
                }

                return result;
            }
        }
    }
}
