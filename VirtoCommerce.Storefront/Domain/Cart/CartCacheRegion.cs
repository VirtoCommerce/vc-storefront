using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Threading;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain
{
    public class CartCacheRegion : CancellableCacheRegion<CartCacheRegion>
    {
        private static readonly ConcurrentDictionary<ShoppingCart, CancellationTokenSource> _cartRegionTokenLookup = new ConcurrentDictionary<ShoppingCart, CancellationTokenSource>();

        public static IChangeToken CreateChangeToken(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }
            var cancellationTokenSource = _cartRegionTokenLookup.GetOrAdd(cart, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

        public static void ExpireCart(ShoppingCart cart)
        {
            if (_cartRegionTokenLookup.TryRemove(cart, out CancellationTokenSource token))
            {
                token.Cancel();
                token.Dispose();
            }
        }

        
    }

}
