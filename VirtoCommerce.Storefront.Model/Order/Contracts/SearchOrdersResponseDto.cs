using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Order.Contracts
{
    public class SearchOrdersResponseDto
    {
        public int TotalCount { get; set; }
        public IList<CustomerOrder> Items { get; set; }
    }

    public class OrdersResponseDto
    {
        public SearchOrdersResponseDto Orders { get; set; }
    }
}
