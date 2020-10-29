using System;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain
{
    public class CustomerCacheRegion : CancellableCacheRegion<CustomerCacheRegion>
    {
        public static IChangeToken CreateChangeToken(string memberId)
        {
            if (memberId == null)
            {
                throw new ArgumentNullException(nameof(memberId));
            }

            return CreateChangeTokenForKey(memberId);
        }

        public static void ExpireMember(string memberId)
        {
            if (!string.IsNullOrEmpty(memberId))
            {
                ExpireTokenForKey(memberId);
            }
        }
    }
}
