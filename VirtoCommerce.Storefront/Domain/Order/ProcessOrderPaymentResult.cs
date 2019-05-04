using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Order;

namespace VirtoCommerce.Storefront.Domain
{
    public class ProcessOrderPaymentResult
    {
        public ProcessPaymentResult OrderProcessingResult { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
