using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain
{
    public class CustomerCacheRegion : CancellableCacheRegion<CustomerCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _memberRegionTokenLookup =
          new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(string memberId)
        {
            if (memberId == null)
            {
                throw new ArgumentNullException(nameof(memberId));
            }
            var cancellationTokenSource = _memberRegionTokenLookup.GetOrAdd(memberId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireMember(string memberId)
        {
            if (!string.IsNullOrEmpty(memberId) && _memberRegionTokenLookup.TryRemove(memberId, out var token))
            {
                token.Cancel();
            }
        }
    }
}
