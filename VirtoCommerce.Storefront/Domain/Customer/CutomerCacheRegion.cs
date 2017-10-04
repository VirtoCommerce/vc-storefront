using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Threading;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Customer;

namespace VirtoCommerce.Storefront.Domain
{
    public class CustomerCacheRegion : CancellableCacheRegion<CustomerCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _customerRegionTokenLookup =
          new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(string customerId)
        {
            if (customerId == null)
            {
                throw new ArgumentNullException(nameof(customerId));
            }
            var cancellationTokenSource = _customerRegionTokenLookup.GetOrAdd(customerId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireCustomer(string customerId)
        {
            if (_customerRegionTokenLookup.TryRemove(customerId, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
