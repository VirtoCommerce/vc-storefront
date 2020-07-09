using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoRest.Core.Utilities;
using GraphQL;
using GraphQL.Client.Abstractions;
using PagedList.Core;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Contracts;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain.Customer
{
    public class ExperienceMemberService : IMemberService
    {
        private readonly IGraphQLClient _client;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ExperienceMemberService(IGraphQLClient client, IWorkContextAccessor workContextAccessor)
        {
            _client = client;
            _workContextAccessor = workContextAccessor;
        }


        public async Task<Contact> CreateContactAsync(Contact contact)
        {
            var request = new GraphQLRequest
            {
                Query = this.CreateContactRequest(),
                Variables = new
                {
                    Command = new
                    {
                        contact.Name,
                        contact.FullName,
                        contact.FirstName,
                        contact.LastName,
                        contact.MiddleName,
                        contact.Salutation,
                        contact.PhotoUrl,
                        contact.TimeZone,
                        contact.DefaultLanguage,
                        organizations = !string.IsNullOrEmpty(contact.OrganizationId) ? new[] { contact.OrganizationId } : contact.OrganizationsIds,
                        Addresses = contact.Addresses.Select(x => x.ToDto())
                    }
                }
            };
            var response = await _client.SendMutationAsync<ContactResponseDto>(request);
            return response.Data?.Contact;
        }

        public async Task<Organization> CreateOrganizationAsync(Organization organization)
        {
            var request = new GraphQLRequest
            {
                Query = this.CreateOrganizationRequest(),
                Variables = new
                {
                    command = new
                    {
                        Name = organization.Name,
                        Addresses = organization.Addresses.Select(x => x.ToDto()).ToList()
                    }
                }
            };
            var response = await _client.SendMutationAsync<CreateOrganizationResponseDto>(request);
            return response.Data?.Organization;
        }

        public async Task DeleteContactAsync(string contactId)
        {
            var request = new GraphQLRequest
            {
                Query = this.DeleteContactRequest(),
                Variables = new
                {
                    Command = new { contactId }
                }
            };
            var response = await _client.SendMutationAsync<DeleteContactResponseDto>(request);
        }

        public async Task<Contact> GetContactByIdAsync(string contactId)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetContactRequest(contactId)
            };
            var response = await _client.SendQueryAsync<ContactResponseDto>(request);

            return response.Data?.Contact;
        }

        /// <summary>
        /// no usages found. Redundant?
        /// </summary>
        public async Task<Organization> GetOrganizationByIdAsync(string organizationId)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetOrganizationRequest(organizationId)
            };
            var response = await _client.SendQueryAsync<GetOrganizationResponseDto>(request);

            return response.Data?.Organization;
        }

        /// <summary>
        /// no usages found. Redundant?
        /// </summary>
        public Vendor[] GetVendorsByIds(Store store, Language language, params string[] vendorIds)
        {
            throw new NotImplementedException();
        }

        public Task<Vendor[]> GetVendorsByIdsAsync(Store store, Language language, params string[] vendorIds)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// no usages found. Redundant? Organization Contacts should be loaded as part of org.
        /// </summary>
        public IPagedList<Contact> SearchOrganizationContacts(OrganizationContactsSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// no usages found. Redundant? Organization Contacts should be loaded as part of org.
        /// </summary>
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
                    Command = new
                    {
                        contactId,
                        addresses = addresses.Select(x => x.ToDto())
                    }
                }
            };
            var response = await _client.SendMutationAsync<ContactResponseDto>(request);

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
                Query = this.UpdateContactRequest("id"),
                // Query = this.UpdateContactRequest(),
                Variables = new
                {
                    //Command = new UpdateContactCommand
                    Command = new
                    {
                        Addresses = contact.Addresses.Select(x => x.ToDto()),
                        contact.DefaultLanguage,
                        //contact.DynamicProperties,
                        contact.Emails,
                        contact.FirstName,
                        contact.FullName,
                        contact.Groups,
                        contact.Id,
                        contact.LastName,
                        contact.MiddleName,
                        contact.Phones,
                        contact.PhotoUrl,
                        contact.Salutation,
                        contact.TimeZone
                    }
                }
            };

            _ = await _client.SendMutationAsync<ContactResponseDto>(request);

            //Invalidate cache
            CustomerCacheRegion.ExpireMember(contact.Id);
        }

        public async Task UpdateOrganizationAsync(Organization organization)
        {
            var request = new GraphQLRequest
            {
                Query = this.UpdateOrganizationRequest("id"),
                Variables = new
                {
                    Command = new
                    {
                        organization.Id,
                        organization.Name,
                        organization.MemberType,
                        organization.Phones,
                        organization.Groups,
                        organization.Emails,
                        organization.Addresses
                    }
                }
            };
            var response = await _client.SendMutationAsync<UpdateOrganizationResponseDto>(request);

            CustomerCacheRegion.ExpireMember(organization.Id);
        }
    }
}
