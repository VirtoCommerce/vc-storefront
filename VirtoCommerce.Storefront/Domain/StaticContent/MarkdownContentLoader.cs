using Markdig;

namespace VirtoCommerce.Storefront.Domain
{
    public class MarkdownContentLoader : StaticContentLoader
    {
        private readonly MarkdownPipeline _markdownPipeline;

        public MarkdownContentLoader(MarkdownPipeline markdownPipeline)
        {
            _markdownPipeline = markdownPipeline;
        }

        public override string PrepareContent(string content)
        {
            return Markdown.ToHtml(base.PrepareContent(content), _markdownPipeline);
        }
    }
}
