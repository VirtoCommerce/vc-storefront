using PagedList.Core;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.QuoteModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Quote.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteModule _quoteApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        public QuoteService(IQuoteModule quoteApi, IWorkContextAccessor workContextAccessor)
        {
            _quoteApi = quoteApi;
            _workContextAccessor = workContextAccessor;
        }

        public IPagedList<QuoteRequest> SearchQuotes(QuoteSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            return Task.Factory.StartNew(() => InnerSearchQuotesAsync(criteria, workContext), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public async Task<IPagedList<QuoteRequest>> SearchQuotesAsync(QuoteSearchCriteria criteria)
        {
            var workContext = _workContextAccessor.WorkContext;
            return await InnerSearchQuotesAsync(criteria, workContext);
        }

        protected virtual async Task<IPagedList<QuoteRequest>> InnerSearchQuotesAsync(QuoteSearchCriteria criteria, WorkContext workContext)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            var result = await _quoteApi.SearchAsync(criteria.ToQuoteSearchCriteriaDto());
            return new StaticPagedList<QuoteRequest>(result.QuoteRequests.Select(x => x.ToQuoteRequest(workContext.CurrentCurrency, workContext.CurrentLanguage)),
                                                     criteria.PageNumber, criteria.PageSize, result.TotalCount.Value);
        }

    }
}
