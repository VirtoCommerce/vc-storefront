using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public interface IContentRestorerFactory
    {
        IContentItemRestorer CreateRestorer(string blobRelativePath, ContentItem item);
    }
}
