using System;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class SecurityCacheRegion : CancellableCacheRegion<SecurityCacheRegion>
    {
        public static IChangeToken CreateChangeToken(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return CreateChangeTokenForKey(userId);
        }

        public static void ExpireUser(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                ExpireTokenForKey(userId);
            }
        }
    }
}
