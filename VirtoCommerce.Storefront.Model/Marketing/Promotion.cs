using System;
using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Marketing
{
    /// <summary>
    /// Represents promotion object
    /// </summary>
    public partial class Promotion
    {
        public string Id { get; set; }

        public ICollection<string> Coupons { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }
    }
}