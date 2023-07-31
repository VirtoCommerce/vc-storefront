using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Security;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using customerDto = VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi.Models;
using platformDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{

    public static partial class MemberConverter
    {
        public static DynamicProperty ToDynamicProperty(this customerDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<platformDto.DynamicObjectProperty>().ToDynamicProperty();
        }


        public static customerDto.DynamicObjectProperty ToCustomerDynamicPropertyDto(this DynamicProperty property)
        {
            return property.ToDynamicPropertyDto().JsonConvert<customerDto.DynamicObjectProperty>();
        }

        public static Address ToAddress(this customerDto.CustomerAddress addressDto)
        {
            return addressDto.JsonConvert<coreDto.Address>().ToAddress();
        }

        public static customerDto.CustomerAddress ToCustomerAddressDto(this Address address)
        {
            return address.ToCoreAddressDto().JsonConvert<customerDto.CustomerAddress>();
        }

        public static Contact ToContact(this UserRegistration userRegistration)
        {
            var result = new Contact
            {
                Name = userRegistration.Name ?? userRegistration.UserName,
                FullName = string.IsNullOrWhiteSpace(userRegistration.FullName) ? string.Join(" ", userRegistration.FirstName, userRegistration.LastName) : userRegistration.FullName,
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                Salutation = userRegistration.Salutation,
                PhotoUrl = userRegistration.PhotoUrl
            };
            if (!string.IsNullOrEmpty(userRegistration.Email))
            {
                result.Emails.Add(userRegistration.Email);
            }
            if (string.IsNullOrEmpty(result.FullName) || string.IsNullOrWhiteSpace(result.FullName))
            {
                result.FullName = userRegistration.Email;
            }
            if (userRegistration.Address != null)
            {
                result.Addresses = new[] { userRegistration.Address };
            }
            return result;
        }

        public static Contact ToContact(this customerDto.Contact contactDto)
        {
            var result = new Contact
            {
                Id = contactDto.Id,
                Name = contactDto.Name,
                MemberType = contactDto.MemberType,
                UserGroups = contactDto.Groups,
                FullName = contactDto.FullName,
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Emails = contactDto.Emails,
                TimeZone = contactDto.TimeZone,
                DefaultLanguage = contactDto.DefaultLanguage,
                OrganizationId = contactDto.Organizations?.FirstOrDefault(),
                OrganizationsIds = contactDto.Organizations,
                Salutation = contactDto.Salutation,
                Status = contactDto.Status,
                PhotoUrl = contactDto.PhotoUrl
            };

            if (contactDto.Addresses != null)
            {
                result.Addresses = contactDto.Addresses.Select(ToAddress).ToList();
            }

            result.DefaultBillingAddress = result.Addresses
                .Where(a => contactDto.DefaultBillingAddressId != null ? a.Id == contactDto.DefaultBillingAddressId : (a.Type & AddressType.Billing) == AddressType.Billing)
                // Stabilize order
                .OrderBy(a => a.Id)
                .FirstOrDefault();
            result.DefaultShippingAddress = result.Addresses
                .Where(a => contactDto.DefaultShippingAddressId != null ? a.Id == contactDto.DefaultShippingAddressId : (a.Type & AddressType.Shipping) == AddressType.Shipping)
                // Stabilize order
                .OrderBy(a => a.Id)
                .FirstOrDefault();

            if (contactDto.Emails != null)
            {
                result.Emails = contactDto.Emails;
            }
            if (!contactDto.DynamicProperties.IsNullOrEmpty())
            {
                result.DynamicProperties = new MutablePagedList<DynamicProperty>(contactDto.DynamicProperties.Select(ToDynamicProperty).ToList());
            }
            if (!contactDto.SecurityAccounts.IsNullOrEmpty())
            {
                result.SecurityAccounts = contactDto.SecurityAccounts.Select(x => new SecurityAccount
                {
                    Id = x.Id,
                    Roles = x.Roles?.Select(role => role.Name).ToList(),
                    IsLockedOut = (x.LockoutEndDateUtc ?? DateTime.MinValue) > DateTime.UtcNow,
                    UserName = x.UserName,
                });
            }
            return result;
        }

        public static Organization ToOrganization(this OrganizationRegistration orgRegistration)
        {
            var organization = new Organization
            {
                Name = orgRegistration.OrganizationName,
            };
            if (organization.Addresses != null)
            {
                organization.Addresses.Add(orgRegistration.Address);
            }
            return organization;
        }

        public static Organization ToOrganization(this customerDto.Organization organizaionDto)
        {
            var result = new Organization
            {
                Id = organizaionDto.Id,
                Name = organizaionDto.Name,
                MemberType = organizaionDto.MemberType,
                UserGroups = organizaionDto.Groups,
            };

            if (organizaionDto.Addresses != null)
            {
                result.Addresses = organizaionDto.Addresses.Select(ToAddress).ToList();
            }

            if (organizaionDto.Phones != null)
            {
                result.Phones = organizaionDto.Phones;
            }

            if (organizaionDto.Emails != null)
            {
                result.Emails = organizaionDto.Emails;
            }

            if (!organizaionDto.DynamicProperties.IsNullOrEmpty())
            {
                result.DynamicProperties = new MutablePagedList<DynamicProperty>(organizaionDto.DynamicProperties.Select(ToDynamicProperty).ToList());
            }

            return result;
        }


        public static customerDto.Contact ToContactDto(this Contact customer)
        {
            var retVal = new customerDto.Contact
            {
                Id = customer.Id,
                Name = customer.Name,
                FirstName = customer.FirstName,
                FullName = customer.FullName,
                LastName = customer.LastName,
                MiddleName = customer.MiddleName,
                Salutation = customer.Salutation,
                Status = customer.Status,
                PhotoUrl = customer.PhotoUrl,
                MemberType = "Contact",
            };
            if (!customer.UserGroups.IsNullOrEmpty())
            {
                retVal.Groups = customer.UserGroups.ToArray();
            }
            if (!customer.Addresses.IsNullOrEmpty())
            {
                retVal.Addresses = new List<customerDto.CustomerAddress>();
                foreach (var address in customer.Addresses)
                {
                    var addressDto = address.ToCustomerAddressDto();
                    if (string.IsNullOrEmpty(addressDto.FirstName))
                    {
                        addressDto.FirstName = customer.FirstName;
                    }
                    if (string.IsNullOrEmpty(addressDto.LastName))
                    {
                        addressDto.LastName = customer.LastName;
                    }
                    if (string.IsNullOrEmpty(addressDto.Email))
                    {
                        addressDto.Email = customer.Email;
                    }
                    retVal.Addresses.Add(addressDto);
                }
            }

            if (customer.DefaultShippingAddress != null)
            {
                retVal.DefaultShippingAddressId = customer.DefaultShippingAddress.Id;
            }

            if (customer.DefaultBillingAddress != null)
            {
                retVal.DefaultBillingAddressId = customer.DefaultBillingAddress.Id;
            }

            if (!customer.Emails.IsNullOrEmpty())
            {
                retVal.Emails = customer.Emails;
            }

            if (customer.OrganizationId != null)
            {
                retVal.Organizations = new List<string>() { customer.OrganizationId };
            }
            if (customer.OrganizationsIds != null)
            {
                retVal.Organizations = customer.OrganizationsIds.Concat(retVal.Organizations ?? Array.Empty<string>()).Distinct().ToArray();
            }

            if (!customer.DynamicProperties.IsNullOrEmpty())
            {
                retVal.DynamicProperties = customer.DynamicProperties.Select(ToCustomerDynamicPropertyDto).ToList();
            }

            return retVal;
        }

        public static customerDto.Organization ToOrganizationDto(this Organization org)
        {
            var retVal = new customerDto.Organization
            {
                Id = org.Id,
                Name = org.Name,
                MemberType = "Organization"
            };

            if (!org.UserGroups.IsNullOrEmpty())
            {
                retVal.Groups = org.UserGroups.ToArray();
            }

            if (!org.Addresses.IsNullOrEmpty())
            {
                retVal.Addresses = org.Addresses.Select(ToCustomerAddressDto).ToList();
            }

            if (!org.Emails.IsNullOrEmpty())
            {
                retVal.Emails = org.Emails;
            }

            if (!org.Phones.IsNullOrEmpty())
            {
                retVal.Phones = org.Phones;
            }

            if (!org.DynamicProperties.IsNullOrEmpty())
            {
                retVal.DynamicProperties = org.DynamicProperties.Select(ToCustomerDynamicPropertyDto).ToList();
            }

            return retVal;
        }

        public static customerDto.Contact ToCoreContactDto(this Contact contact)
        {
            return contact.ToContactDto().JsonConvert<customerDto.Contact>();
        }
    }
}
