using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Stores;
using customerDto = VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public class MemberService : IMemberService
    {
        private readonly ICustomerModule _customerApi;
        private readonly IMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public MemberService(ICustomerModule customerApi, IMemoryCache memoryCache, IApiChangesWatcher changesWatcher)
        {
            _customerApi = customerApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = changesWatcher;
        }

        #region ICustomerService Members
        public virtual Contact GetContactById(string contactId)
        {
            return Task.Factory.StartNew(() => GetContactByIdAsync(contactId), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public virtual async Task<Contact> GetContactByIdAsync(string contactId)
        {
            if(contactId == null)
            {
                throw new ArgumentNullException(nameof(contactId));
            }

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
                if (!dto.Organizations.IsNullOrEmpty())
                {
                    //Load contact organization
                    result.Organization = await GetOrganizationByIdAsync(dto.Organizations.FirstOrDefault());
                }
            }
            return result;
        }
        
        public virtual async Task CreateContactAsync(Contact contact)
        {
            var contactDto = contact.ToCustomerContactDto();
            await _customerApi.CreateContactAsync(contactDto);
        }


        public virtual async Task DeleteContactAsync(string contactId)
        {
            await _customerApi.DeleteContactsAsync(new[] { contactId });
            //Invalidate cache
            CustomerCacheRegion.ExpireMember(contactId);
        }


        public virtual async Task UpdateContactAsync(Contact contact)
        {
            await _customerApi.UpdateContactAsync(contact.ToCustomerContactDto());
            //Invalidate cache
            CustomerCacheRegion.ExpireMember(contact.Id);
        }

        public async Task UpdateContactAddressesAsync(string contactId, IList<Address> addresses)
        {
            var existContact = await GetContactByIdAsync(contactId);
            if (existContact != null)
            {
                await _customerApi.UpdateAddessesAsync(contactId, addresses.Select(x => x.ToCustomerAddressDto()).ToList());

                //Invalidate cache
                CustomerCacheRegion.ExpireMember(existContact.Id);
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

        public async Task<Organization> GetOrganizationByIdAsync(string organizationId)
        {
            Organization result = null;
            var cacheKey = CacheKey.With(GetType(), "GetOrganizationByIdAsync", organizationId);
            var dto = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var organizationDto = await _customerApi.GetOrganizationByIdAsync(organizationId);
                if (organizationDto != null)
                {
                    cacheEntry.AddExpirationToken(CustomerCacheRegion.CreateChangeToken(organizationDto.Id));
                    cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());
                }
                return organizationDto;
            });

            if (dto != null)
            {
                result = dto.ToOrganization();

                //Lazy load organization contacts
                result.Contacts = new MutablePagedList<Contact>((pageNumber, pageSize, sortInfos) =>
                {
                    var criteria = new OrganizationContactsSearchCriteria
                    {
                        OrganizationId = result.Id,
                        PageNumber = pageNumber,
                        PageSize = pageSize                        
                    };
                    if (!sortInfos.IsNullOrEmpty())
                    {
                        criteria.Sort = SortInfo.ToString(sortInfos);
                    }
                    return SearchOrganizationContacts(criteria);
                  
                }, 1, 20);
            }
            return result;
        }

        public Organization GetOrganizationById(string organizationId)
        {
            return Task.Factory.StartNew(() => GetOrganizationByIdAsync(organizationId), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public async Task CreateOrganizationAsync(Organization organization)
        {
            var orgDto = organization.ToOrganizationDto();
            var org = await _customerApi.CreateOrganizationAsync(orgDto);
            organization.Id = org.Id;
        }

        public async Task UpdateOrganizationAsync(Organization organization)
        {
            var orgDto = organization.ToOrganizationDto();
            await _customerApi.UpdateOrganizationAsync(orgDto);
            CustomerCacheRegion.ExpireMember(organization.Id);
        }

        public IPagedList<Contact> SearchOrganizationContacts(OrganizationContactsSearchCriteria criteria)
        {
            return Task.Factory.StartNew(() => SearchOrganizationContactsAsync(criteria), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public async Task<IPagedList<Contact>> SearchOrganizationContactsAsync(OrganizationContactsSearchCriteria criteria)
        {
            var criteriaDto = new customerDto.MembersSearchCriteria
            {
                MemberId = criteria.OrganizationId,
                Skip = (criteria.PageNumber - 1) * criteria.PageSize,
                Take = criteria.PageSize,
                Sort = criteria.Sort,
                SearchPhrase = criteria.SearchPhrase
            };
            
            var searchResult = await _customerApi.SearchAsync(criteriaDto);
            var contacts = _customerApi.GetContactsByIds(searchResult.Results.Select(x => x.Id).ToList()).Select(x => x.ToContact()).ToList();

            return new StaticPagedList<Contact>(contacts, criteria.PageNumber, criteria.PageSize, searchResult.TotalCount.Value);
        }
        #endregion
    }
}

