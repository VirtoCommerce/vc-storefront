namespace VirtoCommerce.Storefront.Domain
{
    public interface IContentItemReaderFactory
    {
        IContentItemReader CreateReader(string blobRelativePath, string content);
    }
}
