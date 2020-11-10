using System;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain
{
    public class SubscriptionCacheRegion : CancellableCacheRegion<SubscriptionCacheRegion>
    {
        public static IChangeToken CreateCustomerSubscriptionChangeToken(string customerId)
        {
            if (customerId == null)
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            return CreateChangeTokenForKey(customerId);
        }

        public static void ExpireCustomerSubscription(string customerId)
        {
            if (!string.IsNullOrEmpty(customerId))
            {
                ExpireTokenForKey(customerId);
            }
        }
    }
}
