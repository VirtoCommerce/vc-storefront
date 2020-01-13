using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain.CustomerReview
{
    public class CustomerReviewCacheRegion : CancellableCacheRegion<CustomerReviewCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _customerCustomerReviewRegionTokenLookup =
        new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateCustomerCustomerReviewChangeToken(string customerId)
        {
            if (customerId == null)
                throw new ArgumentNullException(nameof(customerId));
            var cancellationTokenSource = _customerCustomerReviewRegionTokenLookup.GetOrAdd(customerId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireCustomerCustomerReview(string customerId)
        {
            if (!string.IsNullOrEmpty(customerId) && _customerCustomerReviewRegionTokenLookup.TryRemove(customerId, out var token))
            {
                token.Cancel();
            }
        }
    }
}
