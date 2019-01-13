namespace VirtoCommerce.Storefront.Model.Common
{
    /// <summary>
    /// Represents helper abstraction that used for index access to entity by it key.
    /// </summary>
    public interface IAccessibleByIndexKey
    {
        string IndexKey { get; }
    }
}
