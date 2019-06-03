using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Order;

namespace VirtoCommerce.Storefront.Domain
{
	public class OrderCreatedInfo
	{
		public CustomerOrder Order { get; set; }
		public ProcessPaymentResult OrderProcessingResult { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
	}
}
