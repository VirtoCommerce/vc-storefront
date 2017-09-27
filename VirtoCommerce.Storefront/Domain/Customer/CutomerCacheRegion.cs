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
        private static readonly ConcurrentDictionary<CustomerInfo, CancellationTokenSource> _customerRegionTokenLookup =
          new ConcurrentDictionary<CustomerInfo, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(CustomerInfo customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            var cancellationTokenSource = _customerRegionTokenLookup.GetOrAdd(customer, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireCustomer(CustomerInfo customer)
        {
            if (_customerRegionTokenLookup.TryRemove(customer, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
