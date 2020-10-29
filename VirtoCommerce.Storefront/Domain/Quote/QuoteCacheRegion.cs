using System;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Quote;

namespace VirtoCommerce.Storefront.Domain
{
    public class QuoteCacheRegion : CancellableCacheRegion<QuoteCacheRegion>
    {
        public static IChangeToken CreateChangeToken(QuoteRequest qoute)
        {
            if (qoute == null)
            {
                throw new ArgumentNullException(nameof(qoute));
            }

            return CreateChangeTokenForKey(qoute.GetCacheKey());
        }

        public static void ExpireQuote(QuoteRequest qoute)
        {
            if (qoute != null)
            {
                ExpireTokenForKey(qoute.GetCacheKey());
            }
        }
    }
}
