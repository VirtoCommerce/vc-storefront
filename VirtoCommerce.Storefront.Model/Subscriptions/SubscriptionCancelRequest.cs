namespace VirtoCommerce.Storefront.Model.Subscriptions
{
    public partial class SubscriptionCancelRequest
    {
        public string CustomerId { get; set; }
        public string SubscriptionId { get; set; }
        public string Number { get; set; }
        public string CancelReason { get; set; }
    }
}
