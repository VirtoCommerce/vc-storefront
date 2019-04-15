using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Order.Services
{
    public interface ICustomerOrderService
    {
        Task<IPagedList<CustomerOrder>> SearchOrdersAsync(OrderSearchCriteria criteria);
        IPagedList<CustomerOrder> SearchOrders(OrderSearchCriteria criteria);
        Task<CustomerOrder> GetOrderByNumberAsync(string number);
		Task<CustomerOrder> GetOrderByIdAsync(string number);
    }
}
