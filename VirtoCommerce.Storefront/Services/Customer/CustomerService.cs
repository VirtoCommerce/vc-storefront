using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Order.Services;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Quote.Services;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model.Subscriptions;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using customerDto = VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi.Models;

namespace VirtoCommerce.Storefront.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerModule _customerApi;
        private readonly ICustomerOrderService _orderService;
        private readonly IQuoteService _quoteService;
        private readonly IStoreModule _storeApi;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMemoryCache _memoryCache;

        public CustomerService(ICustomerModule customerApi, ICustomerOrderService orderService,
            IQuoteService quoteService, IStoreModule storeApi, ISubscriptionService subscriptionService, IMemoryCache memoryCache)
        {
            _customerApi = customerApi;
            _orderService = orderService;
            _quoteService = quoteService;
            _storeApi = storeApi;
            _memoryCache = memoryCache;
            _subscriptionService = subscriptionService;
        }

        #region ICustomerService Members

        public virtual async Task<CustomerInfo> GetCustomerByIdAsync(string customerId)
        {
            var cacheKey = CacheKey.With(GetType(),  "GetCustomerByIdAsync", customerId);
            var retVal = await _memoryCache.GetOrCreateAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CustomerCacheRegion.GetChangeToken(customerId));
                //TODO: Make parallels call
                var contact = await _customerApi.GetMemberByIdAsync(customerId);
                CustomerInfo result = null;
                if (contact != null)
                {
                    result = contact.ToCustomerInfo();
                }
               
                return result;
            });

            if (retVal != null)
            {
                retVal = retVal.JsonClone();
                retVal.Orders = LazyLoadCustomerOrders(retVal);
                retVal.QuoteRequests = LazyLoadCustomerQuotes(retVal);
                retVal.Subscriptions = LazyLoadCustomerSubscriptions(retVal);
            }

            return retVal;
        }
        
        public virtual async Task CreateCustomerAsync(CustomerInfo customer)
        {
            var contact = customer.ToCustomerContactDto();
            await _customerApi.CreateContactAsync(contact);
        }
        
        public virtual async Task UpdateCustomerAsync(CustomerInfo customer)
        {
            if (customer.MemberType != typeof(customerDto.Contact).Name)
                throw new HttpRequestException("Can't update customer which member type is not " + typeof(customerDto.Contact).Name);

            var contact = customer.ToCustomerContactDto();
            await _customerApi.UpdateContactAsync(contact);

            //Invalidate cache
            CustomerCacheRegion.ClearCustomerRegion(customer.Id);
        }

        public async Task UpdateAddressesAsync(CustomerInfo customer)
        {
            await _customerApi.UpdateAddessesAsync(customer.Id, customer.Addresses.Select(x => x.ToCustomerAddressDto()).ToList());

            //Invalidate cache
            CustomerCacheRegion.ClearCustomerRegion(customer.Id);
        }

        public virtual async Task<bool> CanLoginOnBehalfAsync(string storeId, string customerId)
        {
            var info = await _storeApi.GetLoginOnBehalfInfoAsync(storeId, customerId);
            return info.CanLoginOnBehalf == true;
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

     

        protected virtual IMutablePagedList<QuoteRequest> LazyLoadCustomerQuotes(CustomerInfo customer)
        {
            Func<int, int, IEnumerable<SortInfo>, IPagedList<QuoteRequest>> quotesGetter = (pageNumber, pageSize, sortInfos) =>
            {
                var quoteSearchCriteria = new QuoteSearchCriteria
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Sort = sortInfos.ToString(),
                    CustomerId = customer.Id                   
                };
                return  _quoteService.SearchQuotes(quoteSearchCriteria);
            };
            return new MutablePagedList<QuoteRequest>(quotesGetter, 1, QuoteSearchCriteria.DefaultPageSize);
        }

        protected virtual IMutablePagedList<CustomerOrder> LazyLoadCustomerOrders(CustomerInfo customer)
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

        protected virtual IMutablePagedList<Subscription> LazyLoadCustomerSubscriptions(CustomerInfo customer)
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

