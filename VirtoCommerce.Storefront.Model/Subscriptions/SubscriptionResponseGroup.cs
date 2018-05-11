using System;

namespace VirtoCommerce.Storefront.Model.Subscriptions
{
    [Flags]
    public enum SubscriptionResponseGroup
    {
        Default = 0,
        WithOrderPrototype = 1,
        WithRlatedOrders = 1 << 1,
        Full = Default | WithOrderPrototype | WithRlatedOrders
    }
}
