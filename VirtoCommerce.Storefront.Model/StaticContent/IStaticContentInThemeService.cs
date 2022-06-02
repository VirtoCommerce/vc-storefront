using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    /// <summary>
    /// Represent a search and rendering static content pages (pages and blogs etc)
    /// </summary>
    public interface IStaticContentInThemeService
    {
        IEnumerable<ContentItem> LoadStoreStaticContent(Store store);
        IEnumerable<ContentItem> LoadStoreStaticTemplates(Store store);
    }
}
