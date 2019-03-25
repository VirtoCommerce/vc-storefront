using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Order;

namespace VirtoCommerce.Storefront.Model.Common.SearchResults
{
    public class CustomerOrderSearchResult
    {
        public CustomerOrderSearchResult()
        {
            Results = new List<CustomerOrder>();
        }

        public int TotalCount { get; set; }
        public ICollection<CustomerOrder> Results { get; set; }

    }
}
