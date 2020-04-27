using System.Linq;
using Markdig;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal class MarkdownVisitor : IContentItemVisitor
    {
        public ContentProcessStage Stage => ContentProcessStage.Content;
        private readonly MarkdownPipeline _markdownPipeline;

        public MarkdownVisitor(MarkdownPipeline markdownPipeline)
        {
            _markdownPipeline = markdownPipeline;
        }

        public bool Suit(ContentItem item)
        {
            return StaticContentItemBuilder.extensions.Any(item.FileName.EndsWith);
        }

        public string ReadContent(string path, string content, ContentItem item)
        {
            item.Content = Markdown.ToHtml(content, _markdownPipeline);
            return item.Content;
        }
    }
}
