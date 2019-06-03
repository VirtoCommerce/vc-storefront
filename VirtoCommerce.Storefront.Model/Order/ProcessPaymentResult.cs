namespace VirtoCommerce.Storefront.Model.Order
{
    public class ProcessPaymentResult
    {
        public PaymentMethod PaymentMethod { get; set; }

        public string NewPaymentStatus { get; set; }

        public string RedirectUrl { get; set; }

        public string HtmlForm { get; set; }

        public bool IsSuccess { get; set; }

        public string Error { get; set; }

        public string OuterId { get; set; }
    }
}
