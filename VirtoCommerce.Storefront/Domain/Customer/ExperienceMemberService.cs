using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoRest.Core.Utilities;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.Http;
using PagedList.Core;
using VirtoCommerce.Storefront.Extensions;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExperienceMemberService(IGraphQLClient client, IWorkContextAccessor workContextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _workContextAccessor = workContextAccessor;
            _httpContextAccessor = httpContextAccessor;
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
                        Addresses = contact.Addresses.Select(x => x.ToDto()),
                        UserId = GetCurrentUserId()
                    }
                }
            };
            var response = await _client.SendMutationAsync<ContactResponseDto>(request);
            response.ThrowExceptionOnError();
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
                        MemberType = nameof(Organization),
                        Addresses = organization.Addresses.Select(x => x.ToDto()).ToList(),
                        UserId = GetCurrentUserId()
                    }
                }
            };
            var response = await _client.SendMutationAsync<OrganizationResponseDto>(request);
            response.ThrowExceptionOnError();
            return response.Data?.Organization;
        }

        public async Task DeleteContactAsync(string contactId)
        {
            var request = new GraphQLRequest
            {
                Query = this.DeleteContactRequest(),
                Variables = new
                {
                    command = new
                    {
                        ContactId = contactId,
                        UserId = GetCurrentUserId()
                    }
                }
            };
            var response = await _client.SendMutationAsync<DeleteContactResponseDto>(request);
            response.ThrowExceptionOnError();
        }

        public async Task<Contact> GetContactByIdAsync(string contactId)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetContactRequest(contactId, GetCurrentUserId())
            };
            var response = await _client.SendQueryAsync<ContactResponseDto>(request);

            if (response.Data?.Contact != null)
            {
                var contact = response.Data.Contact;
                if (contact.OrganizationsIds.Any())
                {
                    contact.OrganizationId = contact.OrganizationsIds.First();
                    contact.Organization = contact.Organizations.FirstOrDefault();
                }

                return contact;
            }

            return null;
        }

        /// <summary>
        /// no usages found. Redundant?
        /// </summary>
        public async Task<Organization> GetOrganizationByIdAsync(string organizationId)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetOrganizationRequest(organizationId, GetCurrentUserId())
            };
            var response = await _client.SendQueryAsync<OrganizationResponseDto>(request);

            return response.Data?.Organization;
        }

        /// <summary>
        /// no usages found. Redundant?
        /// </summary>
        public Vendor[] GetVendorsByIds(Store store, Language language, params string[] vendorIds)
        {
            //TODO:
            return new Vendor[] { };
        }

        public Task<Vendor[]> GetVendorsByIdsAsync(Store store, Language language, params string[] vendorIds)
        {
            //TODO:
            return Task.FromResult(new Vendor[] { });
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
            var request = new GraphQLRequest
            {
                Query = this.OrganizationWithContactsRequest(criteria, GetCurrentUserId())
            };
            var searchResult = await _client.SendQueryAsync<OrganizationContactsResponseDto>(request);

            return new StaticPagedList<Contact>(searchResult.Data.Organization?.Contacts?.Results, criteria.PageNumber, criteria.PageSize, searchResult.Data.Organization?.Contacts?.TotalCount ?? 0);
        }

        public IPagedList<Vendor> SearchVendors(Store store, Language language, string keyword, int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos)
        {
            //TODO:
            return new StaticPagedList<Vendor>(new Vendor[] { }, pageNumber, pageSize, 0);
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
                        addresses = addresses.Select(x => x.ToDto()),
                        UserId = GetCurrentUserId()
                    }
                }
            };
            var response = await _client.SendMutationAsync<ContactResponseDto>(request);
            response.ThrowExceptionOnError();

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
                        Organizations = contact.OrganizationsIds,
                        contact.Phones,
                        contact.PhotoUrl,
                        contact.Salutation,
                        contact.TimeZone,
                        UserId = GetCurrentUserId()
                    }
                }
            };

            var response = await _client.SendMutationAsync<ContactResponseDto>(request);
            response.ThrowExceptionOnError();

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
                        organization.Addresses,
                        UserId = GetCurrentUserId()
                    }
                }
            };
            var response = await _client.SendMutationAsync<OrganizationResponseDto>(request);
            response.ThrowExceptionOnError();

            CustomerCacheRegion.ExpireMember(organization.Id);
        }

        private string GetCurrentUserId()
        {
            var userId = _workContextAccessor.WorkContext?.CurrentUser?.Id;
            if (string.IsNullOrWhiteSpace(userId))
            {
                var principal = _httpContextAccessor.HttpContext.User;
                userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return userId;
        }
    }
}
