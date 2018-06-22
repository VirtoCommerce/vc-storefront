using System;

namespace VirtoCommerce.Storefront.Model.Subscriptions
{
    [Flags]
    public enum SubscriptionResponseGroup
    {
        Default = 1,
        WithChangeLog = 1 << 1,
        WithOrderPrototype = 1 << 2,
        WithRelatedOrders = 1 << 3,
        Full = Default | WithOrderPrototype | WithRelatedOrders | WithChangeLog
    }
}
