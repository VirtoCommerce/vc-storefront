using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.QuoteModuleApi;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Order.Services;
using VirtoCommerce.Storefront.Model.Quote;

namespace VirtoCommerce.Storefront.Services
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
            return Task.Factory.StartNew(() => InnerSearchOrdersAsync(criteria, workContext), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public async Task<IPagedList<CustomerOrder>> SearchOrdersAsync(OrderSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            return await InnerSearchOrdersAsync(criteria, workContext);        }


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
