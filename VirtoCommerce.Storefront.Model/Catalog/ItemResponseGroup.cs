using System;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    [Flags]
    public enum ItemResponseGroup
    {
        None = 0,
        /// <summary>
        /// Only simple product information and properties without meta information
        /// </summary>
        ItemInfo = 1,
        /// <summary>
        /// With images and assets
        /// </summary>
        ItemAssets = 1 << 1,
        /// <summary>
        /// With properties meta information
        /// </summary>
        ItemProperties = 1 << 2,
        /// <summary>
        /// With product associations
        /// </summary>
        ItemAssociations = 1 << 3,
        /// <summary>
        /// With descriptions
        /// </summary>
        ItemEditorialReviews = 1 << 4,
        /// <summary>
        /// With all product variations
        /// </summary>
        Variations = 1 << 5,
        /// <summary>
        /// With product SEO information
        /// </summary>
        Seo = 1 << 6,
        /// <summary>
        /// With outgoing product links to virtual catalog or categories
        /// </summary>
        Links = 1 << 7,
        /// <summary>
        /// With product inventory information
        /// </summary>
        Inventory = 1 << 8,
        /// <summary>
        /// With category outlines
        /// </summary>
        Outlines = 1 << 9,
        /// <summary>
        /// With product referenced associations
        /// </summary>
        ReferencedAssociations = 1 << 10,
        //the bits of this values must not intersect with domain ItemResponseGroup
        ItemWithPrices = 1 << 20,

        ItemWithDiscounts = 1 << 21,

        ItemWithVendor = 1 << 22,

        ItemWithPaymentPlan = 1 << 23,

        ItemSmall = ItemInfo | ItemAssets | Seo | Outlines,
        ItemMedium = ItemSmall | ItemProperties | ItemEditorialReviews,
        ItemLarge = ItemMedium | ItemAssociations | Variations | Inventory |  ItemWithPrices | ItemWithDiscounts | ItemWithVendor  | ItemWithPaymentPlan
    }
}
