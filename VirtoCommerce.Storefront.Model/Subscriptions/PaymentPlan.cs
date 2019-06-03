using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Subscriptions
{
    public class PaymentPlan : Entity
    {
        public PaymentPlan()
        {
            Interval = PaymentInterval.Months;
        }
        /// <summary>
        /// (days, months, years) - billing interval
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        
        public PaymentInterval Interval { get; set; }
        /// <summary>
        /// - to set more customized intervals (every 5 month)
        /// </summary>
        public int IntervalCount { get; set; }
        /// <summary>
        ///  subscription trial period in days 
        /// </summary>
        public int TrialPeriodDays { get; set; }
    }
}
