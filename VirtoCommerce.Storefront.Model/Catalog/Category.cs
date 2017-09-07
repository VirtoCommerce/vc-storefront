using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class Category : Entity, IHasProperties
    {
        public Category()
        {
            Images = new List<Image>();
            Properties = new List<CatalogProperty>();
        }

        public string CatalogId { get; set; }

        //All parents categories
        public IMutablePagedList<Category> Parents { get; set; }
        public string ParentId { get; set; }      

        public string Code { get; set; }

        public string TaxType { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// All parent categories ids concatenated with "/". E.g. (1/21/344)
        /// </summary>
        public string Outline { get; set; }

        public SeoInfo SeoInfo { get; set; }

        /// <summary>
        /// Category main image
        /// </summary>
        public Image PrimaryImage { get; set; }

        public ICollection<Image> Images { get; set; }

        public IMutablePagedList<Product> Products { get; set; }

        /// <summary>
        /// Child categories
        /// </summary>
        public IMutablePagedList<Category> Categories { get; set; }

        #region IHasProperties Members
        public ICollection<CatalogProperty> Properties { get; set; }
        #endregion

        public string SeoPath { get; set; }
        public string Url { get; set; }
    }
}
