using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public static class StoreWorkContextBuilderExtensions
    {
        
        public static Task WithStoresAsync(this IWorkContextBuilder builder, IList<Store> stores, Store curentStore)
        {
            if(stores == null)
            {
                throw new ArgumentNullException(nameof(stores));
            }

            builder.WorkContext.AllStores = stores.ToArray();
            builder.WorkContext.CurrentStore = curentStore ?? stores.FirstOrDefault();

            return Task.CompletedTask;
        }

        public static async Task WithStoresAsync(this IWorkContextBuilder builder, string defaultStoreId)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var storeService = serviceProvider.GetRequiredService<IStoreService>();

            var stores = await storeService.GetAllStoresAsync();

            if (stores.IsNullOrEmpty())
            {
                throw new NoStoresException();
            }

            //Set current store
            //Try first find by store url (if it defined)
            var currentStore = stores.FirstOrDefault(x => builder.HttpContext.Request.Path.StartsWithSegments(new PathString("/" + x.Id)));
            if (currentStore == null)
            {
                currentStore = stores.Where(x => x.IsStoreUrl(builder.WorkContext.RequestUrl)).FirstOrDefault();
            }
            if (currentStore == null && defaultStoreId != null)
            {
                currentStore = stores.FirstOrDefault(x => x.Id.EqualsInvariant(defaultStoreId));
            }
            await builder.WithStoresAsync(stores, currentStore);
        }
    }
}

