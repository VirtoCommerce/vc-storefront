using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class Vendor
    {
        public Vendor()
        {
            Addresses = new List<Address>();
            DynamicProperties = new List<DynamicProperty>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SiteUrl { get; set; }

        public string LogoUrl { get; set; }

        public string GroupName { get; set; }

        /// <summary>
        /// Vendor seo info
        /// </summary>
        public SeoInfo SeoInfo { get; set; }

        public ICollection<Address> Addresses { get; set; }

        public ICollection<DynamicProperty> DynamicProperties { get; set; }

        public IMutablePagedList<Product> Products { get; set; }
    }
}
