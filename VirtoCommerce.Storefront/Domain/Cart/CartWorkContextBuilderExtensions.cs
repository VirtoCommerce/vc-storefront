using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common;
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

        public static Task WithDefaultShoppingCartAsync(this IWorkContextBuilder builder, string cartName, Store store, Model.Customer.CustomerInfo customer, 
                                                        Currency currency, Language language)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var cartBuilder = serviceProvider.GetRequiredService<ICartBuilder>();

            Func<Model.Cart.ShoppingCart> factory = () =>
            {
                Task.Factory.StartNew(() => cartBuilder.LoadOrCreateNewTransientCartAsync(cartName, store, customer, language, currency), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
                return cartBuilder.Cart;
            };
            return builder.WithDefaultShoppingCartAsync(factory);
        }
    }
}
