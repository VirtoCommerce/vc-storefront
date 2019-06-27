using PagedList.Core;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.QuoteModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Order.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly IOrderModule _orderApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        public CustomerOrderService(IOrderModule orderApi, IWorkContextAccessor workContextAccessor)
        {
            _orderApi = orderApi;
            _workContextAccessor = workContextAccessor;
        }

        public IPagedList<CustomerOrder> SearchOrders(OrderSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            return InnerSearchOrdersAsync(criteria, workContext).GetAwaiter().GetResult();
        }

        public async Task<IPagedList<CustomerOrder>> SearchOrdersAsync(OrderSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            return await InnerSearchOrdersAsync(criteria, workContext);
        }

        public async Task<CustomerOrder> GetOrderByNumberAsync(string number)
        {
            var workContext = _workContextAccessor.WorkContext;
            return (await _orderApi.GetByNumberAsync(number))?.ToCustomerOrder(workContext.AllCurrencies, workContext.CurrentLanguage);
        }


        public async Task<CustomerOrder> GetOrderByIdAsync(string id)
        {
            var workContext = _workContextAccessor.WorkContext;
            return (await _orderApi.GetByIdAsync(id))?.ToCustomerOrder(workContext.AllCurrencies, workContext.CurrentLanguage);
        }
		
        protected virtual async Task<IPagedList<CustomerOrder>> InnerSearchOrdersAsync(OrderSearchCriteria criteria, WorkContext workContext)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            var result = await _orderApi.SearchAsync(criteria.ToSearchCriteriaDto());
            return new StaticPagedList<CustomerOrder>(result.CustomerOrders.Select(x => x.ToCustomerOrder(workContext.AllCurrencies, workContext.CurrentLanguage)),
                                                     criteria.PageNumber, criteria.PageSize, result.TotalCount.Value);
        }

    }
}
