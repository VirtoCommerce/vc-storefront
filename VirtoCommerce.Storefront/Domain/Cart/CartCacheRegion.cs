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
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _cartSearchRegionLookup = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static IChangeToken CreateCustomerChangeToken(string customerId)
        {
            if (customerId == null)
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            var cancellationTokenSource = _cartSearchRegionLookup.GetOrAdd(customerId, new CancellationTokenSource());
            return new CompositeChangeToken(new[] { CreateChangeToken(), new CancellationChangeToken(cancellationTokenSource.Token) });
        }

   
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
            if (cart != null)
            {
                if (_cartRegionTokenLookup.TryRemove(cart, out CancellationTokenSource token))
                {
                    token.Cancel();
                    token.Dispose();
                }
                ExpireCustomerCarts(cart.CustomerId);
            }
        }

        public static void ExpireCustomerCarts(string customerId)
        {
            if (_cartSearchRegionLookup.TryRemove(customerId, out CancellationTokenSource token))
            {
                token.Cancel();
                token.Dispose();
            }
        }

    }

}
