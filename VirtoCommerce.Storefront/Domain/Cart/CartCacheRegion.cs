using System;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Domain
{
    public class CartCacheRegion : CancellableCacheRegion<CartCacheRegion>
    {
        public static IChangeToken CreateCustomerChangeToken(string customerId)
        {
            if (customerId == null)
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            return CreateChangeTokenForKey(customerId);
        }


        public static IChangeToken CreateChangeToken(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }

            return CreateChangeTokenForKey(cart.GetCacheKey());
        }

        public static void ExpireCart(ShoppingCart cart)
        {
            if (cart != null)
            {
                ExpireTokenForKey(cart.GetCacheKey());
                ExpireCustomerCarts(cart.CustomerId);
            }
        }

        public static void ExpireCustomerCarts(string customerId)
        {
            ExpireTokenForKey(customerId);
        }

    }

}
