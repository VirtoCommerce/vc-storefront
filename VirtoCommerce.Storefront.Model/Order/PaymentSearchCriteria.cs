using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Order
{
    public partial class PaymentSearchCriteria : PagedSearchCriteria
    {
        public static int DefaultPageSize { get; set; } = 20;

        public PaymentSearchCriteria()
            : base(new NameValueCollection(), DefaultPageSize)
        {
        }
        public PaymentSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }

        public string Sort { get; set; }
        public string Keyword { get; set; }      
        public string Status { get; set; }
        public IList<string> Statuses { get; set; }
        public string[] StoreIds { get; set; }
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
