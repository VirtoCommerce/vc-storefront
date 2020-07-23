using System.Threading.Tasks;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Order.Services
{
    public interface ICustomerOrderService
    {
        Task<IPagedList<CustomerOrder>> SearchOrdersAsync(OrderSearchCriteria criteria);
        IPagedList<CustomerOrder> SearchOrders(OrderSearchCriteria criteria);
        Task<CustomerOrder> GetOrderByNumberAsync(string number);
		Task<CustomerOrder> GetOrderByIdAsync(string id);
        Task<CustomerOrder> CreateOrderFromCartAsync(string cartId);
        Task UpdateOrderAsync(CustomerOrder order);
        Task ChangeOrderStatusAsync(string orderId, string status);
        Task CancelPayment(PaymentIn payment);
        Task ConfirmPayment(PaymentIn payment);
    }
}
