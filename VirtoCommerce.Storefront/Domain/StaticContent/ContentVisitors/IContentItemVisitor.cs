using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public interface IContentItemVisitor
    {
        ContentProcessStage Stage { get; }
        bool Suit(ContentItem item);
        string ReadContent(string path, string content, ContentItem item);
    }
}
