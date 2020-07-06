using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoRest.Core.Utilities;
using GraphQL;
using GraphQL.Client.Abstractions;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Commands;
using VirtoCommerce.Storefront.Model.Customer.Contracts;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain.Customer
{
    public class ExperienceMemberService : IMemberService
    {
        private readonly IGraphQLClient _client;
        private readonly IWorkContextAccessor _workContextAccessor;

        private readonly ICustomerModule _customerApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;
        private readonly MemberService _memberService;

        public ExperienceMemberService(IGraphQLClient client, IWorkContextAccessor workContextAccessor, ICustomerModule customerApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher)
        {
            _client = client;
            _workContextAccessor = workContextAccessor;
            _customerApi = customerApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
            _memberService = new MemberService(customerApi, memoryCache, apiChangesWatcher);
        }


        public async Task<Contact> CreateContactAsync(Contact contact)
        {
            // throw new NotImplementedException();
            return await _memberService.CreateContactAsync(contact);
        }

        public Task<Organization> CreateOrganizationAsync(Organization organization)
        {
            throw new NotImplementedException();
        }

        public Task DeleteContactAsync(string contactId)
        {
            throw new NotImplementedException();
        }

        public async Task<Contact> GetContactByIdAsync(string contactId)
        {
            //throw new NotImplementedException();
            return await _memberService.GetContactByIdAsync(contactId);
        }

        public Task<Organization> GetOrganizationByIdAsync(string organizationId)
        {
            throw new NotImplementedException();
        }

        public Vendor[] GetVendorsByIds(Store store, Language language, params string[] vendorIds)
        {
            throw new NotImplementedException();
        }

        public Task<Vendor[]> GetVendorsByIdsAsync(Store store, Language language, params string[] vendorIds)
        {
            throw new NotImplementedException();
        }

        public IPagedList<Contact> SearchOrganizationContacts(OrganizationContactsSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public async Task<IPagedList<Contact>> SearchOrganizationContactsAsync(OrganizationContactsSearchCriteria criteria)
        {
            //var criteriaDto = new customerDto.MembersSearchCriteria
            //{
            //    MemberId = criteria.OrganizationId,
            //    Skip = (criteria.PageNumber - 1) * criteria.PageSize,
            //    Take = criteria.PageSize,
            //    Sort = criteria.Sort,
            //    SearchPhrase = criteria.SearchPhrase,
            //    ObjectType = "Member"
            //};

            var request = new GraphQLRequest
            {
                Query = this.SearchOrganizationContacts(criteria.PageSize, (criteria.PageNumber - 1) * criteria.PageSize),
                Variables = new
                {
                    criteria = new
                    {
                        organizationId = criteria.OrganizationId,
                        searchPhrase = criteria.SearchPhrase
                    }
                }
            };
            var searchResult = await _client.SendQueryAsync<SearchMemberResponseDto>(request);
            //var searchResult = await _customerApi.SearchMemberAsync(criteriaDto);
            //var contacts = _customerApi.GetContactsByIds(searchResult.Results.Select(x => x.Id).ToList()).Select(x => x.ToContact()).ToList();
            //var contacts = searchResult.Data?.Items.Select(x => x.ToContact()).ToList();

            return new StaticPagedList<Contact>(searchResult.Data?.Items, criteria.PageNumber, criteria.PageSize, searchResult.Data.TotalCount);
        }

        public IPagedList<Vendor> SearchVendors(Store store, Language language, string keyword, int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateContactAddressesAsync(string contactId, IList<Address> addresses)
        {
            addresses.ForEach(x => x.Email ??= _workContextAccessor.WorkContext.CurrentUser?.Email);

            var request = new GraphQLRequest
            {
                Query = this.UpdateContactAddressesRequest(),
                Variables = new
                {
                    id = contactId,
                    addresses = addresses.Select(x => x.ToDto())
                }
            };
            var response = await _client.SendMutationAsync<Contact>(request);

            if (response.Data != null)
            {
                //Invalidate cache
                CustomerCacheRegion.ExpireMember(contactId);
            }
        }

        public async Task UpdateContactAsync(Contact contact)
        {
            var request = new GraphQLRequest
            {
                Query = this.UpdateContactRequest(),
                Variables = new
                {
                    Command = new UpdateContactCommand
                    {
                        Id = _workContextAccessor.WorkContext.CurrentUser.Id,
                        //StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        //CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        //Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        //Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        //CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        //Payment = payment.ToDto()
                    }
                }
            };

            var response = await _client.SendMutationAsync<UpdateContactResponseDto>(request);

            //Invalidate cache
            CustomerCacheRegion.ExpireMember(contact.Id);
        }

        public async Task UpdateOrganizationAsync(Organization organization)
        {
            var request = new GraphQLRequest
            {
                Query = this.UpdateOrganizationRequest(nameof(Member.Id)),
                Variables = new
                {
                    command = organization.ToDto()
                }
            };
            await _client.SendMutationAsync<Organization>(request);

            CustomerCacheRegion.ExpireMember(organization.Id);
        }
    }
}
