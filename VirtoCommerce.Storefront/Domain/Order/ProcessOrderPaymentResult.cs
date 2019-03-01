using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public class ProcessOrderPaymentResult
    {
        public ProcessPaymentResult OrderProcessingResult { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
