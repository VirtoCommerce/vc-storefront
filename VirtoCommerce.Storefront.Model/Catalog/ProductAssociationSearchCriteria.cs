using System;
using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class ProductAssociationSearchCriteria : PagedSearchCriteria
    {
        public static int DefaultPageSize { get; set; } = 20;

        public ProductAssociationSearchCriteria()
            : base(new NameValueCollection(), DefaultPageSize)
        {
        }

        public ProductAssociationSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }
        //Required
        public string ProductId { get; set; }
        public string Sort { get; set; }

        public ItemResponseGroup ResponseGroup { get; set; } = ItemResponseGroup.ItemInfo;
        public string Group { get; set; }

    }
}
