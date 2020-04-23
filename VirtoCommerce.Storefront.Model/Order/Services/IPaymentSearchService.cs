using System.Threading.Tasks;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Order.Services
{
    public interface IPaymentSearchService
    {
        Task<IPagedList<PaymentIn>> SearchPaymentsAsync(PaymentSearchCriteria criteria);
    }
}
