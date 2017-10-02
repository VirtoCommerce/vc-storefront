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
        private static readonly ConcurrentDictionary<Contact, CancellationTokenSource> _customerRegionTokenLookup =
          new ConcurrentDictionary<Contact, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(Contact customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            var cancellationTokenSource = _customerRegionTokenLookup.GetOrAdd(customer, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireCustomer(Contact customer)
        {
            if (_customerRegionTokenLookup.TryRemove(customer, out CancellationTokenSource token))
            {
                token.Cancel();
            }
        }
    }
}
