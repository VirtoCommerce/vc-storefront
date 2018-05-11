using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.Recommendations
{
    public partial class UsageEvent
    {
        public DateTime? CreatedDate { get; set; }
        public string CustomerId { get; set; }
        public string StoreId { get; set; }
        public string ItemId { get; set; }
        public string EventType { get; set; }
        public string Id { get; set; }

    }
}
