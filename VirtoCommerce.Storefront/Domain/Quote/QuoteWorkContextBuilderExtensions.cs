using Microsoft.Extensions.DependencyInjection;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                 quoteRequestBuilder.GetOrCreateNewTransientQuoteRequestAsync(store, user, language, currency).GetAwaiter().GetResult();
                 return quoteRequestBuilder.QuoteRequest;
             };
            return builder.WithQuotesAsync(factory);
        }

        public static Task WithUserQuotesAsync(this IWorkContextBuilder builder, IMutablePagedList<Model.Quote.QuoteRequest> quotes)
        {
            builder.WorkContext.CurrentUser.QuoteRequests = quotes;
            return Task.CompletedTask;
        }

        public static Task WithUserQuotesAsync(this IWorkContextBuilder builder)
        {
            if (builder.WorkContext.CurrentUser != null)
            {
                var serviceProvider = builder.HttpContext.RequestServices;
                var quoteService = serviceProvider.GetRequiredService<IQuoteService>();

                Func<int, int, IEnumerable<SortInfo>, NameValueCollection, IPagedList<Model.Quote.QuoteRequest>> factory = (pageNumber, pageSize, sortInfos, @params) =>
                {
                    var quoteSearchCriteria = new Model.Quote.QuoteSearchCriteria
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Sort = sortInfos?.ToString(),
                        CustomerId = builder.WorkContext.CurrentUser.Id
                    };
                    if (@params != null)
                    {
                        quoteSearchCriteria.CopyFrom(@params);
                    }
                    return quoteService.SearchQuotes(quoteSearchCriteria);
                };
                return builder.WithUserQuotesAsync(new MutablePagedList<Model.Quote.QuoteRequest>(factory, 1, Model.Quote.QuoteSearchCriteria.DefaultPageSize));
            }
            return Task.CompletedTask;
        }

    }
}
