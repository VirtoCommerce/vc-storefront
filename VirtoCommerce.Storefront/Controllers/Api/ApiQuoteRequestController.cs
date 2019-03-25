using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Common.SearchResults;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("")]
    public class ApiQuoteRequestController : StorefrontControllerBase
    {
        private readonly IQuoteRequestBuilder _quoteRequestBuilder;
        private readonly ICartBuilder _cartBuilder;
        private readonly ICatalogService _catalogService;
        private readonly IQuoteService _quoteService;

        public ApiQuoteRequestController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ICartBuilder cartBuilder, IQuoteRequestBuilder quoteRequestBuilder, ICatalogService catalogService, IQuoteService quoteService)
            : base(workContextAccessor, urlBuilder)
        {
            _quoteRequestBuilder = quoteRequestBuilder;
            _cartBuilder = cartBuilder;
            _catalogService = catalogService;
            _quoteService = quoteService;
        }

        // POST: storefrontapi/quoterequests/search
        [HttpPost("quoterequests/search")]
        [ResponseType(typeof(QuoteSearchResult))]
        public ActionResult<QuoteSearchResult> QuoteSearch([FromBody] QuoteSearchCriteria criteria)
        {
            if (WorkContext.CurrentUser.IsRegisteredUser)
            {
                //allow search only within self quotes
                criteria.CustomerId = WorkContext.CurrentUser.Id;
                var result = _quoteService.SearchQuotes(criteria);
                return new QuoteSearchResult
                {
                    Results = result.ToArray(),
                    TotalCount = result.TotalItemCount
                };
            }
            return NoContent();
        }

        // GET: storefrontapi/quoterequests/{number}/itemscount
        [HttpGet("quoterequests/{number}/itemscount")]
        public async Task<ActionResult<QuoteItemsCount>> GetItemsCount(string number)
        {
            await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);

            var quoteRequest = _quoteRequestBuilder.QuoteRequest;
            EnsureQuoteRequestBelongsToCurrentCustomer(quoteRequest);

            return new QuoteItemsCount
            {
                Id = quoteRequest.Id,
                ItemsCount = quoteRequest.ItemsCount
            };
        }

        // GET: storefrontapi/quoterequests/{number}
        [HttpGet("quoterequests/{number}")]
        public async Task<ActionResult<QuoteRequest>> Get(string number)
        {
            var builder = await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);
            var quoteRequest = builder.QuoteRequest;

            EnsureQuoteRequestBelongsToCurrentCustomer(quoteRequest);

            quoteRequest.Customer = WorkContext.CurrentUser;

            return quoteRequest;
        }

        // GET: storefrontapi/quoterequest/current
        [HttpGet("quoterequest/current")]
        public ActionResult<QuoteRequest> GetCurrent()
        {
            EnsureQuoteRequestBelongsToCurrentCustomer(WorkContext.CurrentQuoteRequest.Value);
            return WorkContext.CurrentQuoteRequest.Value;
        }

        // POST: storefrontapi/quoterequests/current/items
        [HttpPost("quoterequests/current/items")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem([FromBody] AddQuoteItem addQuoteItem)
        {
            EnsureQuoteRequestBelongsToCurrentCustomer(WorkContext.CurrentQuoteRequest.Value);
            _quoteRequestBuilder.TakeQuoteRequest(WorkContext.CurrentQuoteRequest.Value);

            using (await AsyncLock.GetLockByKey(GetAsyncLockQuoteKey(_quoteRequestBuilder.QuoteRequest.Id)).LockAsync())
            {
                var products = await _catalogService.GetProductsAsync(new[] { addQuoteItem.ProductId }, ItemResponseGroup.ItemInfo | ItemResponseGroup.ItemWithPrices);
                if (products != null && products.Any())
                {
                    _quoteRequestBuilder.AddItem(products.First(), addQuoteItem.Quantity);
                    await _quoteRequestBuilder.SaveAsync();
                }
            }

            return Ok();
        }

        // DELETE: storefrontapi/quoterequest/{number}/items/{itemId}
        [HttpDelete("quoterequests/{number}/items/{itemId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveItem(string number, string itemId)
        {
            await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);

            using (await AsyncLock.GetLockByKey(GetAsyncLockQuoteKey(_quoteRequestBuilder.QuoteRequest.Id)).LockAsync())
            {
                _quoteRequestBuilder.RemoveItem(itemId);
                await _quoteRequestBuilder.SaveAsync();
            }
            return Ok();
        }

        // POST: storefrontapi/quoterequest/{number}/submit
        [HttpPost("quoterequests/{number}/submit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(string number, [FromBody] QuoteRequestFormModel quoteForm)
        {
            await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);

            EnsureQuoteRequestBelongsToCurrentCustomer(_quoteRequestBuilder.QuoteRequest);

            using (await AsyncLock.GetLockByKey(WorkContext.CurrentQuoteRequest.Value.Id).LockAsync())
            {
                _quoteRequestBuilder.Update(quoteForm).Submit();
                await _quoteRequestBuilder.SaveAsync();
            }

            return Ok();
        }

        // POST: storefrontapi/quoterequest/{number}/reject
        [HttpPost("quoterequests/{number}/reject")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Reject(string number)
        {
            await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);

            EnsureQuoteRequestBelongsToCurrentCustomer(_quoteRequestBuilder.QuoteRequest);

            using (await AsyncLock.GetLockByKey(_quoteRequestBuilder.QuoteRequest.Id).LockAsync())
            {
                _quoteRequestBuilder.Reject();
                await _quoteRequestBuilder.SaveAsync();
            }
            return Ok();
        }

        // PUT: storefrontapi/quoterequest/{number}/update
        [HttpPut("quoterequests/{number}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(string number, [FromBody] QuoteRequestFormModel quoteRequest)
        {
            await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);

            EnsureQuoteRequestBelongsToCurrentCustomer(_quoteRequestBuilder.QuoteRequest);

            using (await AsyncLock.GetLockByKey(_quoteRequestBuilder.QuoteRequest.Id).LockAsync())
            {
                _quoteRequestBuilder.Update(quoteRequest);
                await _quoteRequestBuilder.SaveAsync();
            }

            return Ok();
        }

        // POST: storefrontapi/quoterequests/{number}/totals
        [HttpPost("quoterequests/{number}/totals")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<QuoteRequestTotals>> CalculateTotals(string number, [FromBody] QuoteRequestFormModel quoteRequest)
        {
            await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);

            EnsureQuoteRequestBelongsToCurrentCustomer(_quoteRequestBuilder.QuoteRequest);

            //Apply user changes without saving
            _quoteRequestBuilder.Update(quoteRequest);
            await _quoteRequestBuilder.CalculateTotalsAsync();

            return _quoteRequestBuilder.QuoteRequest.Totals;
        }

        // POST: storefrontapi/quoterequests/{number}/confirm
        [HttpPost("quoterequests/{number}/confirm")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Confirm([FromRoute]string number, [FromBody] QuoteRequestFormModel quoteRequest)
        {
            await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);

            EnsureQuoteRequestBelongsToCurrentCustomer(_quoteRequestBuilder.QuoteRequest);

            _quoteRequestBuilder.Update(quoteRequest).Confirm();
            await _quoteRequestBuilder.SaveAsync();

            await _cartBuilder.TakeCartAsync(WorkContext.CurrentCart.Value);
            await _cartBuilder.FillFromQuoteRequestAsync(_quoteRequestBuilder.QuoteRequest);
            await _cartBuilder.SaveAsync();

            return Ok();
        }

        private static string GetAsyncLockQuoteKey(string quoteId)
        {
            return "quote-request:" + quoteId;
        }

        private void EnsureQuoteRequestBelongsToCurrentCustomer(QuoteRequest quote)
        {
            if (WorkContext.CurrentUser.Id != quote.CustomerId)
            {
                throw new StorefrontException("Requested quote not belongs to user " + WorkContext.CurrentUser.UserName);
            }
        }
    }
}
