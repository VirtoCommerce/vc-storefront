using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public interface IContentItemRestorer
    {
        void FulfillContent(IContentItemReader reader, ContentItem contentItem);
    }
}
