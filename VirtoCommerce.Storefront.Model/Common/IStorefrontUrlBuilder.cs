using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Common
{
    public interface IStorefrontUrlBuilder
    {
        string ToAppAbsolute(string virtualPath);
        string ToAppAbsolute(string virtualPath, Store store, Language language);
        string ToStoreAbsolute(string virtualPath, Store store = null, Language language = null);
        string ToAppRelative(string virtualPath);
        string ToAppRelative(string virtualPath, Store store, Language language);
        string ToLocalPath(string virtualPath);
    }
}
