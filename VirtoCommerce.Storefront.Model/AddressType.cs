using System;

namespace VirtoCommerce.Storefront.Model
{
    [Flags]
    public enum AddressType
    {
        Billing = 1,
        Shipping = 2,
        BillingAndShipping = Billing | Shipping
    }
}