using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public static class QuoteWorkContextBuilderExtensions
    {
        public static Task WithQuotesAsync(this IWorkContextBuilder builder, Func<Model.Quote.QuoteRequest> factory)
        {
            builder.WorkContext.CurrentQuoteRequest = new Lazy<Model.Quote.QuoteRequest>(factory);
            return Task.CompletedTask;
        }

        public static Task WithQuotesAsync(this IWorkContextBuilder builder, Store store, User user, Currency currency, Language language)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var quoteRequestBuilder = serviceProvider.GetRequiredService<IQuoteRequestBuilder>();

            Func<Model.Quote.QuoteRequest> factory = () =>
             {
                 Task.Factory.StartNew(() => quoteRequestBuilder.GetOrCreateNewTransientQuoteRequestAsync(store, user, language, currency),
                                                                                                          CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
                 return quoteRequestBuilder.QuoteRequest;
             };
            return builder.WithQuotesAsync(factory);
        }

    }
}
