using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Subscriptions
{
    public partial class SubscriptionSearchCriteria : PagedSearchCriteria
    {
        private static int _defaultPageSize = 20;

        public static int DefaultPageSize
        {
            get { return _defaultPageSize; }
            set { _defaultPageSize = value; }
        }
        public SubscriptionSearchCriteria()
            : base(new NameValueCollection(), _defaultPageSize)
        {
        }

        public SubscriptionSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }
        public string Number { get; set; }
        public string CustomerId { get; set; }
        public string Sort { get; set; }
        public DateTime? ModifiedSinceDate { get; set; }
        public SubscriptionResponseGroup ResponseGroup { get; set; } = SubscriptionResponseGroup.WithOrderPrototype;
    }
}
