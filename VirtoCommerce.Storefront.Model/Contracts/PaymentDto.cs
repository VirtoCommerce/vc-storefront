namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class PaymentDto
    {
        public string OuterId { get; set; }
        public string PaymentGatewayCode { get; set; }
        public AddressDto BillingAddress { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }

        public decimal Amount { get; set; }
    }
}
