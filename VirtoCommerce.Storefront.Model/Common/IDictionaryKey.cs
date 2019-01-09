namespace VirtoCommerce.Storefront.Model.Common
{
    /// <summary>
    /// Represents helper abstraction that used for access by key for objects stored in IDictionary.
    /// </summary>
    public interface IDictionaryKey
    {
        string Key { get; }
    }
}
