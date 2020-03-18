using System;
using System.Linq;
using System.Threading.Tasks;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Order.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class PaymentSearchService : IPaymentSearchService
    {
        private readonly IOrderModule _orderApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        public PaymentSearchService(IOrderModule orderApi, IWorkContextAccessor workContextAccessor)
        {
            _orderApi = orderApi;
            _workContextAccessor = workContextAccessor;
        }

        public async Task<IPagedList<PaymentIn>> SearchPaymentsAsync(PaymentSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            var workContext = _workContextAccessor.WorkContext;
            var result = await _orderApi.SearchPaymentsAsync(criteria.ToPaymentSearchCriteriaDto());
            return new StaticPagedList<PaymentIn>(result.Results.Select(x => x.ToOrderInPayment(workContext.AllCurrencies, workContext.CurrentLanguage)),
                                                     criteria.PageNumber, criteria.PageSize, result.TotalCount.Value);
        }
       
    }
}
