using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain.Lists
{
    public class WishlistCacheRegion : CancellableCacheRegion<QuoteCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _whishlistSearchRegionLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateSearchResultsChangeToken(string customerId)
        {
            if (customerId == null)
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            var cancellationTokenSource = _whishlistSearchRegionLookup.GetOrAdd(customerId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireSearchResults(string customerId)
        {
            if (_whishlistSearchRegionLookup.TryRemove(customerId, out CancellationTokenSource token))
            {
                token.Cancel();
                token.Dispose();
            }
        }
    }
}
