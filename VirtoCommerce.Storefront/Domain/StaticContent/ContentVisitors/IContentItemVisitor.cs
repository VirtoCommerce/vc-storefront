using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal interface IContentItemVisitor
    {
        bool Suit(ContentItem item);
        ContentItem Parse(string path, string content, ContentItem item);
    }
}
