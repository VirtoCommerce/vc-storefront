using System;

namespace VirtoCommerce.Storefront.Model.Common
{
    public class DateTimeRange
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public bool IncludeFrom { get; set; }

        public bool IncludeTo { get; set; }
    }
}
