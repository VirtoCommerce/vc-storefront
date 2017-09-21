using CacheManager.Core;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.QuoteModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Order.Events;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Quote.Events;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Stores;
using quoteModel = VirtoCommerce.Storefront.AutoRestClients.QuoteModuleApi.Models;

namespace VirtoCommerce.Storefront.Builders
{
    public class QuoteRequestBuilder : IQuoteRequestBuilder, IEventHandler<UserLoginEvent>
    {
        private readonly IQuoteModule _quoteApi;
        private readonly ICacheManager<object> _cacheManager;
        private readonly IEventPublisher _publisher;

        private QuoteRequest _quoteRequest;
        private const string _quoteRequestCacheRegion = "QuoteRequestRegion";

        public QuoteRequestBuilder(IQuoteModule quoteApi, ICacheManager<object> cacheManager, IEventPublisher publisher)
        {
            _quoteApi = quoteApi;
            _cacheManager = cacheManager;
            _publisher = publisher;
        }

        #region IQuoteRequestBuilder Members

        public async Task<IQuoteRequestBuilder> LoadQuoteRequestAsync(string number, Language language, IEnumerable<Currency> availCurrencies)
        {
            var quoteRequest = await _quoteApi.GetByIdAsync(number);
            if (quoteRequest == null)
            {
                throw new StorefrontException("Quote request for number " + number + " not found");
            }
            _quoteRequest = quoteRequest.ToQuoteRequest(availCurrencies, language);

            return this;
        }

        public IQuoteRequestBuilder TakeQuoteRequest(QuoteRequest quoteRequest)
        {
            _quoteRequest = quoteRequest;

            return this;
        }

        public async Task<IQuoteRequestBuilder> GetOrCreateNewTransientQuoteRequestAsync(Store store, CustomerInfo customer, Language language, Currency currency)
        {
            var cacheKey = GetQuoteRequestCacheKey(store.Id, customer.Id);

            _quoteRequest = await _cacheManager.GetAsync(cacheKey, _quoteRequestCacheRegion, async () =>
            {
                var activeQuoteSearchCriteria = new quoteModel.QuoteRequestSearchCriteria
                {
                    Tag = "actual",
                    CustomerId = customer.Id,
                    StoreId = store.Id
                };

                var searchResult = await _quoteApi.SearchAsync(activeQuoteSearchCriteria);

                var quoteRequest = searchResult.QuoteRequests.Select(x => x.ToQuoteRequest(store.Currencies, language)).FirstOrDefault();
                if (quoteRequest == null)
                {
                    quoteRequest = new QuoteRequest(currency, language)
                    {
                        Currency = currency,
                        CustomerId = customer.Id,
                        Language = language,
                        Status = "New",
                        StoreId = store.Id,
                        Tag = "actual"
                    };

                    quoteRequest.CustomerName = customer.IsRegisteredUser
                    ? string.Join(" ", customer.FirstName, customer.LastName)
                    : StorefrontConstants.AnonymousUsername;
                }
                else
                {
                    quoteRequest = (await _quoteApi.GetByIdAsync(quoteRequest.Id)).ToQuoteRequest(store.Currencies, language);
                }

                quoteRequest.Customer = customer;

                return quoteRequest;
            });

            return this;
        }

        public IQuoteRequestBuilder Submit()
        {
            if (_quoteRequest.ItemsCount == 0)
            {
                throw new StorefrontException("Can not submit an empty quote request");
            }

            if (_quoteRequest.Status == "Ordered")
            {
                throw new StorefrontException("Can not submit an ordered quote request");
            }

            _quoteRequest.Tag = null;
            _quoteRequest.Status = "Processing";

            return this;
        }

        public IQuoteRequestBuilder Reject()
        {
            if (_quoteRequest.Status == "New" || _quoteRequest.Status == "Ordered")
            {
                throw new StorefrontException("Can not reject new or ordered quote request");
            }

            _quoteRequest.Tag = null;
            _quoteRequest.Status = "Rejected";

            return this;
        }

        public IQuoteRequestBuilder Confirm()
        {
            if (_quoteRequest.Status != "Proposal sent")
            {
                throw new StorefrontException("Can not confirm an quote request");
            }

            _quoteRequest.Tag = null;
            _quoteRequest.Status = "Ordered";

            return this;
        }

        public IQuoteRequestBuilder AddItem(Product product, long quantity)
        {
            _quoteRequest.Items.Add(product.ToQuoteItem(quantity));

            return this;
        }

        public IQuoteRequestBuilder RemoveItem(string quoteItemId)
        {
            var quoteItem = _quoteRequest.Items.FirstOrDefault(i => i.Id == quoteItemId);
            if (quoteItem != null)
            {
                _quoteRequest.Items.Remove(quoteItem);
            }

            return this;
        }

        public IQuoteRequestBuilder Update(QuoteRequestFormModel quoteRequest)
        {
            _cacheManager.Remove(GetQuoteRequestCacheKey(_quoteRequest.StoreId, _quoteRequest.CustomerId), _quoteRequestCacheRegion);

            _quoteRequest.Comment = quoteRequest.Comment;
            _quoteRequest.Status = quoteRequest.Status;
            _quoteRequest.Tag = quoteRequest.Tag;

            _quoteRequest.Addresses.Clear();
            if (quoteRequest.BillingAddress != null)
            {
                _quoteRequest.Addresses.Add(quoteRequest.BillingAddress);
            }
            if (quoteRequest.ShippingAddress != null)
            {
                _quoteRequest.Addresses.Add(quoteRequest.ShippingAddress);
            }

            if (quoteRequest.Items != null)
            {
                foreach (var item in quoteRequest.Items)
                {
                    var existingItem = _quoteRequest.Items.FirstOrDefault(i => i.Id == item.Id);
                    if (existingItem != null)
                    {
                        existingItem.Comment = item.Comment;
                        existingItem.SelectedTierPrice = new TierPrice(new Money(item.SelectedTierPrice.Price, _quoteRequest.Currency), item.SelectedTierPrice.Quantity);
                        existingItem.ProposalPrices.Clear();
                        foreach (var proposalPrice in item.ProposalPrices)
                        {
                            existingItem.ProposalPrices.Add(new TierPrice(new Money(proposalPrice.Price, _quoteRequest.Currency), proposalPrice.Quantity));
                        }
                    }
                }
            }

            return this;
        }

        public async Task<IQuoteRequestBuilder> MergeFromOtherAsync(QuoteRequest quoteRequest)
        {
            _quoteRequest.Comment = quoteRequest.Comment;

            foreach (var quoteItem in quoteRequest.Items)
            {
                _quoteRequest.Items.Add(quoteItem);
            }

            if (quoteRequest.Addresses != null && quoteRequest.Addresses.Any())
            {
                _quoteRequest.Addresses = quoteRequest.Addresses;
            }

            await _quoteApi.DeleteAsync(new[] { quoteRequest.Id }.ToList());
            _cacheManager.Remove(GetQuoteRequestCacheKey(_quoteRequest.StoreId, _quoteRequest.CustomerId), _quoteRequestCacheRegion);

            return this;
        }

        public async Task SaveAsync()
        {
            _cacheManager.Remove(GetQuoteRequestCacheKey(_quoteRequest.StoreId, _quoteRequest.CustomerId), _quoteRequestCacheRegion);

            var quoteDto = _quoteRequest.ToQuoteRequestDto();
            if (_quoteRequest.IsTransient())
            {
                await _quoteApi.CreateAsync(quoteDto);
            }
            else
            {
                await _quoteApi.UpdateAsync(quoteDto);
            }

            await _publisher.Publish(new QuoteRequestUpdatedEvent(_quoteRequest));
        }

        public QuoteRequest QuoteRequest
        {
            get
            {
                return _quoteRequest;
            }
        }

        public async Task<IQuoteRequestBuilder> CalculateTotalsAsync()
        {
            var result = await _quoteApi.CalculateTotalsAsync(_quoteRequest.ToQuoteRequestDto());
            _quoteRequest.Totals = result.Totals.ToQuoteTotals(_quoteRequest.Currency);
            return this;
        }

        #endregion

        #region IObserver<UserLoginEvent> Members

        /// <summary>
        /// Merge anonymous user quote to newly log in user quote by log in event
        /// </summary>
        /// <param name="userLoginEvent"></param>
        public virtual async Task Handle(UserLoginEvent @event)
        {
            //If previous user was anonymous and it has not empty cart need merge anonymous cart to personal
            if (@event.WorkContext.CurrentStore.QuotesEnabled && !@event.WorkContext.CurrentCustomer.IsRegisteredUser
                 && @event.WorkContext.CurrentQuoteRequest != null && @event.WorkContext.CurrentQuoteRequest.Value.Items.Any())
            {
                await GetOrCreateNewTransientQuoteRequestAsync(@event.WorkContext.CurrentStore, @event.User, @event.WorkContext.CurrentLanguage, @event.WorkContext.CurrentCurrency);
                await MergeFromOtherAsync(@event.WorkContext.CurrentQuoteRequest.Value);
                await SaveAsync();
            }
        }

        #endregion

        private string GetQuoteRequestCacheKey(string storeId, string customerId)
        {
            return string.Format(CultureInfo.InvariantCulture, "QuoteRequest-{0}-{1}", storeId, customerId);
        }
    }
}
