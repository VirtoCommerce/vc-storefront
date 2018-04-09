using System;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DateTimeRange
    {
        public DateTime? EarlierDate { get; set; }

        public DateTime? LaterDate { get; set; }

        public bool IncludeEarlier { get; set; }

        public bool IncludeLater { get; set; }
    }
}