using System;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    [Flags]
    public enum CategoryResponseGroup
    {
        None = 0,
        Info = 1,
        WithImages = 1 << 1,
        WithProperties = 1 << 2,
        WithLinks = 1 << 3,
        WithSeo = 1 << 4,
        WithParents = 1 << 5,
        WithOutlines = 1 << 6,
        Small = Info | WithImages | WithSeo | WithOutlines,
        Full = Info | WithImages | WithProperties | WithLinks | WithSeo | WithParents | WithOutlines
    }
}
