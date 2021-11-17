namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public interface IStaticContentLoaderFactory
    {
        IStaticContentLoader CreateLoader(ContentItem contentItem);
    }
}
