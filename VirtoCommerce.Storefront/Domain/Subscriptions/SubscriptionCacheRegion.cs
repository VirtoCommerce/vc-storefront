using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain
{
    public class SubscriptionCacheRegion : CancellableCacheRegion<SubscriptionCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _customerSubscriptionRegionTokenLookup =
        new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateCustomerSubscriptionChangeToken(string customerId)
        {
            if (customerId == null)
            {
                throw new ArgumentNullException(nameof(customerId));
            }
            var cancellationTokenSource = _customerSubscriptionRegionTokenLookup.GetOrAdd(customerId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireCustomerSubscription(string customerId)
        {
            if (!string.IsNullOrEmpty(customerId) && _customerSubscriptionRegionTokenLookup.TryRemove(customerId, out var token))
            {
                token.Cancel();
            }
        }
    }
}
