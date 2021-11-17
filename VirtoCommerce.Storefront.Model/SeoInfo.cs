using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    /// <summary>
    /// Represent SEO information and contains common SEO fields  
    /// </summary>
    public partial class SeoInfo : ValueObject, IHasLanguage
    {
        public string MetaDescription { get; set; }

        public string Slug { get; set; }

        public string MetaKeywords { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        #region IHasLanguage Members
        public Language Language { get; set; }
        #endregion

    }
}
