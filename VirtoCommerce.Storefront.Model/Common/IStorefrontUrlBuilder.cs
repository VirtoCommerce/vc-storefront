using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Common
{
    public interface IStorefrontUrlBuilder
    {
        string ToAppAbsolute(string virtualPath);
        string ToAppAbsolute(string virtualPath, Store store, Language language);
        string ToAppRelative(string virtualPath);
        string ToAppRelative(string virtualPath, Store store, Language language);
        string ToLocalPath(string virtualPath);

        /// <summary>
        /// Does the same as <see cref="ToStoreRelativeUrl(string, Store)"/> for the <see cref="WorkContext.CurrentStore"/>.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        string ToStoreRelativeUrl(string virtualPath);
        /// <summary>
        /// Gets store relative path by removing store url path (<see cref="Store.Url"/> and <see cref="Store.SecureUrl"/>) for the given store from the virtual path.
        /// <para/>
        /// Examples:
        /// <para/>For storeUrl="http://domain.com/store" (store path is "/store")) and virtualPath="/store/account" returns "/account".
        /// <para/>For storeUrl without significant path (means path = "/") returns virtualPath without changes.
        /// <para/>Returns absolute Urls without changes.
        /// </summary>
        /// <param name="virtualPath">Path to get relative path from.</param>
        /// <param name="store">Store to which path is relative.</param>
        /// <returns>Store relative url.</returns>
        string ToStoreRelativeUrl(string virtualPath, Store store);
    }
}
