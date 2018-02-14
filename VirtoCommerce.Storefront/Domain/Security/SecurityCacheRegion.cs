using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class SecurityCacheRegion : CancellableCacheRegion<SecurityCacheRegion>
    {
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _securityCacheRegionTokenLookup =
         new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            var cancellationTokenSource = _securityCacheRegionTokenLookup.GetOrAdd(userId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireUser(string userId)
        {
            if (_securityCacheRegionTokenLookup.TryRemove(userId, out var token))
            {
                token.Cancel();
            }
        }
    }
}
