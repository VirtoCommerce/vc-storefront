using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;

namespace VirtoCommerce.Storefront.Model
{
    public partial class Vendor : Member
    {
        public string Description { get; set; }

        public string SiteUrl { get; set; }

        public string LogoUrl { get; set; }

        public string GroupName { get; set; }

        /// <summary>
        /// Vendor seo info
        /// </summary>
        public SeoInfo SeoInfo { get; set; }

        public IMutablePagedList<Product> Products { get; set; }
    }
}
