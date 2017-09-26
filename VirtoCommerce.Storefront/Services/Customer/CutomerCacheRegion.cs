using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Threading;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Services
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
            var cancellationTokenSource = new CancellationTokenSource();
            var token = _customerRegionTokenLookup.GetOrAdd(customerId, cancellationTokenSource);    
            return new CompositeChangeToken(new[] { GetChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ClearCustomerRegion(string customerId)
        {
            if (_customerRegionTokenLookup.TryGetValue(customerId, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
