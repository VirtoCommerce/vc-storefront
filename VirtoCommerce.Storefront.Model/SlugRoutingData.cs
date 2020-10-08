using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model
{
    public class SlugRoutingData
    {
        public string ObjectType { get; set; }
        public string ObjectId { get; set; }
        public object ObjectInstance { get; set; }
        public string SeoPath { get; set; }
    }
}
