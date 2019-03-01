using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
	public class OrderCreatedInfo
	{
		public CustomerOrder Order { get; set; }
		public ProcessPaymentResult OrderProcessingResult { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
	}
}
