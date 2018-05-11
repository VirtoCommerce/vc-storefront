using System.Collections.Generic;
using System.Runtime.Serialization;
using DotLiquid;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    [DataContract]
    public partial class Vendor : Drop
    {
        /// <summary>
        /// Returns the handle of a vendor.
        /// </summary>
        [DataMember]
        public string Handle { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string SiteUrl { get; set; }

        [DataMember]
        public string LogoUrl { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public IMutablePagedList<Address> Addresses { get; set; }

        [DataMember]
        public IList<DynamicProperty> DynamicProperties { get; set; }

        [DataMember]
        public IMutablePagedList<Product> Products { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
