using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Subscriptions
{
    public partial class SubscriptionCancelRequest
    {
        public string Number { get; set; }
        public string CancelReason { get; set; }
    }
}
