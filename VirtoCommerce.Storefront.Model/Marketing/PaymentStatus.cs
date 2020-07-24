namespace VirtoCommerce.Storefront.Model.Marketing
{
    public enum PaymentStatus
    {
        New = 0,
        Pending = 1,
        Authorized = 2,
        Paid = 3,
        PartiallyRefunded = 4,
        Refunded = 5,
        Voided = 6,
        Custom = 7,
        Cancelled = 8,
        Declined = 9,
        Error = 10
    }
}
