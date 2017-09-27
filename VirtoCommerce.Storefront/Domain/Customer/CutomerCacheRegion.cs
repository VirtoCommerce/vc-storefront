using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Threading;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain
{
    public class CustomerCacheRegion : CancellableCacheRegion<CustomerCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _customerRegionTokenLookup =
          new ConcurrentDictionary<string, CancellationTokenSource>(StringComparer.OrdinalIgnoreCase);

        public static IChangeToken GetChangeToken(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }
            var cancellationTokenSource = _customerRegionTokenLookup.GetOrAdd(customerId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { GetChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ClearCustomerRegion(string customerId)
        {
            if (_customerRegionTokenLookup.TryRemove(customerId, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
