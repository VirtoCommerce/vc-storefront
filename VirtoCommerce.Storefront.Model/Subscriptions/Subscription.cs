using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Order;

namespace VirtoCommerce.Storefront.Model.Subscriptions
{
    public partial class Subscription : CustomerOrder
    {
      
        public Subscription(Currency currency)
            : base(currency)
        {
            Balance = new Money(currency);
        }

        /// <summary>
        /// Subscription actual balance
        /// </summary>
        public Money Balance { get; set; }

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

        /// <summary>
        /// List of all orders  created on the basis of the subscription
        /// </summary>
        public IList<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();

        /// <summary>
        /// Date the most recent update to this subscription started.
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// The date the subscription ended
        /// </summary>
        public DateTime? EndDate { get; set; }

        //If the subscription has a trial, the beginning of that trial.
        public DateTime? TrialSart { get; set; }
        // If the subscription has a trial, the end of that trial.
        public DateTime? TrialEnd { get; set; }

        //Start of the current period that the subscription has been ordered for
        public DateTime? CurrentPeriodStart { get; set; }

        //End of the current period that the subscription has been ordered for. At the end of this period, a new order will be created.
        public DateTime? CurrentPeriodEnd { get; set; }

    }
}
