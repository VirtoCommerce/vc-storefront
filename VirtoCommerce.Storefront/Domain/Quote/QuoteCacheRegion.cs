using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Threading;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Quote;

namespace VirtoCommerce.Storefront.Domain
{
    public class QuoteCacheRegion : CancellableCacheRegion<QuoteCacheRegion>
    {
        private static readonly ConcurrentDictionary<QuoteRequest, CancellationTokenSource> _quoteRegionLookup = new ConcurrentDictionary<QuoteRequest, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(QuoteRequest qoute)
        {
            if (qoute == null)
            {
                throw new ArgumentNullException(nameof(qoute));
            }
            var cancellationTokenSource = _quoteRegionLookup.GetOrAdd(qoute, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireQuote(QuoteRequest qoute)
        {
            if (_quoteRegionLookup.TryRemove(qoute, out CancellationTokenSource token))
            {
                token.Cancel();
                token.Dispose();
            }
        }
    }
}
