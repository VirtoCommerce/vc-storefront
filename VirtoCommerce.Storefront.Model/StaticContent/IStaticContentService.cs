using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    /// <summary>
    /// Represent a search and rendering static content pages (pages and blogs etc)
    /// </summary>
    public interface IStaticContentService
    {
        IEnumerable<ContentItem> LoadStoreStaticContent(Store store);
    }
}
