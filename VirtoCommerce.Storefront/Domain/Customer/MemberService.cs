using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Order.Events;
using VirtoCommerce.Storefront.Model.Order.Services;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Security.Events;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model.Subscriptions;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using customerDto = VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public class MemberService : IMemberService
    {
        private readonly ICustomerModule _customerApi;
        private readonly ICustomerOrderService _orderService;
        private readonly IQuoteService _quoteService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public MemberService(ICustomerModule customerApi, ICustomerOrderService orderService,
            IQuoteService quoteService, ISubscriptionService subscriptionService, IMemoryCache memoryCache, IApiChangesWatcher changesWatcher)
        {
            _customerApi = customerApi;
            _orderService = orderService;
            _quoteService = quoteService;
            _memoryCache = memoryCache;
            _subscriptionService = subscriptionService;
            _apiChangesWatcher = changesWatcher;
        }

        #region ICustomerService Members
        public virtual Contact GetContactById(string contactId)
        {
            return Task.Factory.StartNew(() => GetContactByIdAsync(contactId), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public virtual async Task<Contact> GetContactByIdAsync(string contactId)
        {
            Contact result = null;
            var cacheKey = CacheKey.With(GetType(), "GetContactByIdAsync", contactId);
            var dto = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {              
                var contactDto = await _customerApi.GetContactByIdAsync(contactId);
                if (contactDto != null)
                {
                    cacheEntry.AddExpirationToken(CustomerCacheRegion.CreateChangeToken(contactDto.Id));
                    cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());
                }
                return contactDto;
            });

            if (dto != null)
            {
                result = dto.ToContact();
                result.Orders = LazyLoadCustomerOrders(result);
                result.QuoteRequests = LazyLoadCustomerQuotes(result);
                result.Subscriptions = LazyLoadCustomerSubscriptions(result);
            }
            return result;
        }
        
        public virtual async Task CreateContactAsync(Contact contact)
        {
            var contactDto = contact.ToCustomerContactDto();
            await _customerApi.CreateContactAsync(contactDto);
        }
        
        public virtual async Task UpdateContactAsync(string contactId, ContactUpdateInfo contactUpdateInfo)
        {
            var existContact = await GetContactByIdAsync(contactId);
            if (existContact != null)
            {
                existContact.FirstName = contactUpdateInfo.FirstName;
                existContact.LastName = contactUpdateInfo.LastName;
                existContact.Email = contactUpdateInfo.Email;

                var contactDto = existContact.ToCustomerContactDto();
                await _customerApi.UpdateContactAsync(contactDto);

                //Invalidate cache
                CustomerCacheRegion.ExpireCustomer(existContact.Id);
            }
        }

        public async Task UpdateContactAddressesAsync(string contactId, IList<Address> addresses)
        {
            var existContact = await GetContactByIdAsync(contactId);
            if (existContact != null)
            {
                await _customerApi.UpdateAddessesAsync(contactId, addresses.Select(x => x.ToCustomerAddressDto()).ToList());

                //Invalidate cache
                CustomerCacheRegion.ExpireCustomer(existContact.Id);
            }
        }       

        public virtual async Task<Vendor[]> GetVendorsByIdsAsync(Store store, Language language, params string[] vendorIds)
        {
            return (await _customerApi.GetVendorsByIdsAsync(vendorIds)).Select(x => x.ToVendor(language, store)).ToArray();
        }

        public virtual Vendor[] GetVendorsByIds(Store store, Language language, params string[] vendorIds)
        {
            var retVal = _customerApi.GetVendorsByIds(vendorIds).Select(x => x.ToVendor(language, store)).ToArray();
            return retVal;
        }

        public virtual IPagedList<Vendor> SearchVendors(Store store, Language language, string keyword, int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos)
        {
            // TODO: implement indexed search for vendors
            //TODO: Add caching for vendors
            var criteria = new customerDto.MembersSearchCriteria
            {
                SearchPhrase = keyword,
                DeepSearch = true,
                Skip = (pageNumber - 1) * pageSize,
                Take = pageSize
            };

            if (!sortInfos.IsNullOrEmpty())
            {
                criteria.Sort = SortInfo.ToString(sortInfos);
            }
            var vendorSearchResult = _customerApi.SearchVendors(criteria);
            var vendors = vendorSearchResult.Vendors.Select(x => x.ToVendor(language, store));       
            return new StaticPagedList<Vendor>(vendors, pageNumber, pageSize, vendorSearchResult.TotalCount.Value);
        }
        #endregion      

        protected virtual IMutablePagedList<QuoteRequest> LazyLoadCustomerQuotes(Contact customer)
        {
            Func<int, int, IEnumerable<SortInfo>, IPagedList<QuoteRequest>> quotesGetter = (pageNumber, pageSize, sortInfos) =>
            {
                var quoteSearchCriteria = new QuoteSearchCriteria
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Sort = sortInfos?.ToString(),
                    CustomerId = customer.Id                   
                };
                return  _quoteService.SearchQuotes(quoteSearchCriteria);
            };
            return new MutablePagedList<QuoteRequest>(quotesGetter, 1, QuoteSearchCriteria.DefaultPageSize);
        }

        protected virtual IMutablePagedList<CustomerOrder> LazyLoadCustomerOrders(Contact customer)
        {
            var orderSearchcriteria = new OrderSearchCriteria
            {
                CustomerId = customer.Id             
            };

            Func<int, int, IEnumerable<SortInfo>, IPagedList<CustomerOrder>> ordersGetter = (pageNumber, pageSize, sortInfos) =>
            {
                orderSearchcriteria.PageNumber = pageNumber;
                orderSearchcriteria.PageSize = pageSize;
                return _orderService.SearchOrders(orderSearchcriteria);
            };
            return new MutablePagedList<CustomerOrder>(ordersGetter, 1, OrderSearchCriteria.DefaultPageSize);
        }

        protected virtual IMutablePagedList<Subscription> LazyLoadCustomerSubscriptions(Contact customer)
        {
            var subscriptionSearchcriteria = new SubscriptionSearchCriteria
            {
                CustomerId = customer.Id             
            };

            Func<int, int, IEnumerable<SortInfo>, IPagedList<Subscription>> subscriptionGetter = (pageNumber, pageSize, sortInfos) =>
            {
                subscriptionSearchcriteria.PageNumber = pageNumber;
                subscriptionSearchcriteria.PageSize = pageSize;          
                return _subscriptionService.SearchSubscription(subscriptionSearchcriteria);
            };
            return new MutablePagedList<Subscription>(subscriptionGetter, 1, SubscriptionSearchCriteria.DefaultPageSize);
        }

    }
}

