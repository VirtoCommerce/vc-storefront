using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal interface IContentItemVisitor
    {
        bool Suit(ContentItem item);
        string ReadContent(string path, string content, ContentItem item);
    }
}
