using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public interface IStaticContentItemBuilder
    {
        ContentItem BuildFrom(string baseStoreContentPath, string blobRelativePath, string content);
    }
}
