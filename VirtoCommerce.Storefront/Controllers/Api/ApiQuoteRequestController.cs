using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiQuoteRequestController : StorefrontControllerBase
    {
        private readonly IQuoteRequestBuilder _quoteRequestBuilder;
        private readonly ICartBuilder _cartBuilder;
        private readonly ICatalogService _catalogService;

        public ApiQuoteRequestController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ICartBuilder cartBuilder, IQuoteRequestBuilder quoteRequestBuilder, ICatalogService catalogService)
            : base(workContextAccessor, urlBuilder)
        {
            _quoteRequestBuilder = quoteRequestBuilder;
            _cartBuilder = cartBuilder;
            _catalogService = catalogService;
        }

        // GET: storefrontapi/quoterequests/{number}/itemscount
        [HttpGet]
        public async Task<ActionResult> GetItemsCount(string number)
        {
            await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);

            var quoteRequest = _quoteRequestBuilder.QuoteRequest;
            EnsureQuoteRequestBelongsToCurrentCustomer(quoteRequest);

            return Json(new { Id = quoteRequest.Id, ItemsCount = quoteRequest.ItemsCount });
        }

        // GET: storefrontapi/quoterequests/{number}
        [HttpGet]
        public async Task<ActionResult> Get(string number)
        {
            var builder = await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);
            var quoteRequest = builder.QuoteRequest;

            EnsureQuoteRequestBelongsToCurrentCustomer(quoteRequest);

            quoteRequest.Customer = WorkContext.CurrentUser;

            return Json(quoteRequest);
        }

        // GET: storefrontapi/quoterequest/current
        [HttpGet]
        public ActionResult GetCurrent()
        {
            EnsureQuoteRequestBelongsToCurrentCustomer(WorkContext.CurrentQuoteRequest.Value);
            return Json(WorkContext.CurrentQuoteRequest.Value);
        }

        // POST: storefrontapi/quoterequests/current/items
        [HttpPost]
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
        [HttpDelete]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpPut]
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
        [HttpPost]
        public async Task<ActionResult> CalculateTotals(string number, [FromBody] QuoteRequestFormModel quoteRequest)
        {
            await _quoteRequestBuilder.LoadQuoteRequestAsync(number, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);

            EnsureQuoteRequestBelongsToCurrentCustomer(_quoteRequestBuilder.QuoteRequest);

            //Apply user changes without saving
            _quoteRequestBuilder.Update(quoteRequest);
            await _quoteRequestBuilder.CalculateTotalsAsync();

            return Json(_quoteRequestBuilder.QuoteRequest.Totals);
        }

        // POST: storefrontapi/quoterequests/{number}/confirm
        [HttpPost]
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
