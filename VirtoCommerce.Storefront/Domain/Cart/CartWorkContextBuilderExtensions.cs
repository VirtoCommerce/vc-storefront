using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public static class CartWorkContextBuilderExtensions
    {
        public static Task WithDefaultShoppingCartAsync(this IWorkContextBuilder builder, Func<Model.Cart.ShoppingCart> factory)
        {
            builder.WorkContext.CurrentCart = new Lazy<Model.Cart.ShoppingCart>(factory);
            return Task.CompletedTask;
        }

        public static Task WithDefaultShoppingCartAsync(this IWorkContextBuilder builder, string cartName, Store store, User user, 
                                                        Currency currency, Language language)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var cartBuilder = serviceProvider.GetRequiredService<ICartBuilder>();

            Func<Model.Cart.ShoppingCart> factory = () =>
            {
                cartBuilder.LoadOrCreateNewTransientCart(cartName, store, user, language, currency);
                return cartBuilder.Cart;
            };
            return builder.WithDefaultShoppingCartAsync(factory);
        }
    }
}
