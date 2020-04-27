using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal class PageContentVisitor : IContentItemVisitor
    {
        public ContentProcessStage Stage => ContentProcessStage.Content;
        public bool Suit(ContentItem item)
        {
            return item.FileName.EndsWith(".page");
        }

        public string ReadContent(string path, string content, ContentItem item)
        {
            item.Content = content;
            return content;
        }
    }
}
