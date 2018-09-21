using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class ProductAssociation : ValueObject
    {
        public ProductAssociation()
        {
            Tags = new List<string>();
        }
        public string ProductId { get; set; }
        /// <summary>
        /// Associated product
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Association type Related, Associations, Up-Sales etc.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Association priority 0 min 
        /// </summary>
        public int Priority { get; set; }

        public int? Quantity { get; set; }

        public IList<string> Tags { get; set; }
    }
}
