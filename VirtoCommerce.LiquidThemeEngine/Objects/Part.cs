using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/objects/part
    /// Each part returned by the paginate.parts array represents a link in the pagination's navigation.
    /// </summary>
    public partial class Part : ValueObject
    {
        /// <summary>
        /// Returns true if the part is a link, returns false if it is not.
        /// </summary>
        public bool IsLink { get; set; }

        /// <summary>
        /// Returns the title of the part.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Returns the URL of the part.
        /// </summary>
        public string Url { get; set; }
    }
}
