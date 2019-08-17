using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Order
{
    public partial class OrderSearchCriteria : PagedSearchCriteria
    {
        private static int _defaultPageSize = 20;

        public static int DefaultPageSize
        {
            get { return _defaultPageSize; }
            set { _defaultPageSize = value; }
        }

        public OrderSearchCriteria()
            : base(new NameValueCollection(), _defaultPageSize)
        {
        }
        public OrderSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }

        public string Sort { get; set; }

        public string CustomerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public IList<string> Statuses { get; set; }

    }
}

