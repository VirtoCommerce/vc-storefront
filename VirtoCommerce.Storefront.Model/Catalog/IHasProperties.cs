using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public interface IHasProperties
    {
        IList<CatalogProperty> Properties { get; }
    }
}
