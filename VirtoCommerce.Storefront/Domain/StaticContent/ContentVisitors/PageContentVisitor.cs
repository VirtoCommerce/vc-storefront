using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal class PageContentVisitor : IContentItemVisitor
    {
        public bool Suit(ContentItem item)
        {
            return item.FileName.EndsWith(".page");
        }

        public ContentItem Parse(string path, string content, ContentItem item)
        {
            item.Content = content;
            return item;
        }
    }
}
