using System;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DateTimeRange
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public bool IncludeFrom { get; set; }

        public bool IncludeTo { get; set; }
    }
}
