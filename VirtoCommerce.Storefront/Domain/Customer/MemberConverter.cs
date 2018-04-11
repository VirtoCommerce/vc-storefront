using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Stores;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using customerDto = VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    
    public static partial class MemberConverter
    {
        private static readonly char[] _nameSeparator = { ' ' };

        public static  DynamicProperty ToDynamicProperty(this customerDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<coreDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public static customerDto.DynamicObjectProperty ToCustomerDynamicPropertyDto(this DynamicProperty property)
        {
            return property.ToDynamicPropertyDto().JsonConvert<customerDto.DynamicObjectProperty>();
        }

        public static Address ToAddress(this customerDto.Address addressDto)
        {
            return addressDto.JsonConvert<coreDto.Address>().ToAddress();
        }

        public static customerDto.Address ToCustomerAddressDto(this Address address)
        {
            return address.ToCoreAddressDto().JsonConvert<customerDto.Address>();
        }

        public static Vendor ToVendor(this customerDto.Vendor vendorDto, Language currentLanguage, Store store)
        {
            Vendor result = null;

            if (vendorDto != null)
            {
                result = new Vendor
                {
                    Id = vendorDto.Id,
                    Name = vendorDto.Name,
                    Description = vendorDto.Description,
                    LogoUrl = vendorDto.LogoUrl,
                    SiteUrl = vendorDto.SiteUrl,
                    GroupName = vendorDto.GroupName
                };

                if (!vendorDto.SeoInfos.IsNullOrEmpty())
                {
                    var seoInfoDto = vendorDto.SeoInfos.Select(x => x.JsonConvert<coreDto.SeoInfo>())
                        .GetBestMatchingSeoInfos(store, currentLanguage)
                        .FirstOrDefault();

                    if (seoInfoDto != null)
                    {
                        result.SeoInfo = seoInfoDto.ToSeoInfo();
                    }
                }

                if (result.SeoInfo == null)
                {
                    result.SeoInfo = new SeoInfo
                    {
                        Title = vendorDto.Name,
                        Slug = string.Concat("/vendor/", result.Id)
                    };
                }

                if (vendorDto.Addresses != null)
                {
                    result.Addresses = vendorDto.Addresses.Select(ToAddress).ToList();
                }

                if (vendorDto.DynamicProperties != null)
                {
                    result.DynamicProperties = vendorDto.DynamicProperties.Select(ToDynamicProperty).ToList();
                }
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

     
        public static Contact ToContact(this UserRegistration userRegistration)
        {
            var result = new Contact
            {
                Name = userRegistration.Name ?? userRegistration.UserName,
                FullName = string.Join(" ", userRegistration.FirstName, userRegistration.LastName),
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
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
                MemberType = contactDto.MemberType,
                UserGroups = contactDto.Groups,
                FullName = contactDto.FullName,
                FirstName = contactDto.FirstName,
                LastName = contactDto.LastName,
                Emails = contactDto.Emails,
                TimeZone = contactDto.TimeZone,
                DefaultLanguage = contactDto.DefaultLanguage,
                OrganizationId = contactDto.Organizations?.FirstOrDefault()
            };

            if (contactDto.Addresses != null)
            {
                result.Addresses = contactDto.Addresses.Select(ToAddress).ToList();
            }

            result.DefaultBillingAddress = result.Addresses.FirstOrDefault(a => (a.Type & AddressType.Billing) == AddressType.Billing);
            result.DefaultShippingAddress = result.Addresses.FirstOrDefault(a => (a.Type & AddressType.Shipping) == AddressType.Shipping);
       
            if (contactDto.Emails != null)
            {
                result.Emails = contactDto.Emails;              
            }
            if (!contactDto.DynamicProperties.IsNullOrEmpty())
            {
                result.DynamicProperties = contactDto.DynamicProperties.Select(ToDynamicProperty).ToList();
            }
            if(!contactDto.SecurityAccounts.IsNullOrEmpty())
            {
                result.SecurityAccounts = contactDto.SecurityAccounts.Select(x => new SecurityAccount
                {
                    Id = x.Id,
                    Roles = x.Roles?.Select(role => role.Name).ToList(),
                    IsLockedOut =  x.LockoutEndDateUtc != null ? x.LockoutEndDateUtc.Value > DateTime.UtcNow : false,
                    UserName = x.UserName,
                });
            }
            return result;
        }

        public static Organization ToOrganization(this customerDto.Organization organizaionDto)
        {
            var result = new Organization
            {
                Id = organizaionDto.Id,
                Name = organizaionDto.Name,
                MemberType = organizaionDto.MemberType,
                UserGroups = organizaionDto.Groups,             
                Emails = organizaionDto.Emails
            };

            if (organizaionDto.Addresses != null)
            {
                result.Addresses = organizaionDto.Addresses.Select(ToAddress).ToList();
            }
    
            if (organizaionDto.Emails != null)
            {
                result.Emails = organizaionDto.Emails;
            }
            if (!organizaionDto.DynamicProperties.IsNullOrEmpty())
            {
                result.DynamicProperties = organizaionDto.DynamicProperties.Select(ToDynamicProperty).ToList();
            }

            return result;
        }


        public static customerDto.Contact ToContactDto(this Contact customer)
        {
            var retVal = new customerDto.Contact
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                FullName = customer.FullName,
                LastName = customer.LastName,
                MiddleName = customer.MiddleName,
                MemberType = "Contact",
            };
            if (!customer.UserGroups.IsNullOrEmpty())
            {
                retVal.Groups = customer.UserGroups.ToArray();
            }
            if (!customer.Addresses.IsNullOrEmpty())
            {
                retVal.Addresses = customer.Addresses.Select(ToCustomerAddressDto).ToList();
            }
            if (!customer.Emails.IsNullOrEmpty())
            {
                retVal.Emails = customer.Emails;
            }

            //Support only one organization then
            if(customer.OrganizationId != null)
            {
                retVal.Organizations = new List<string>() { customer.OrganizationId };
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
            return retVal;
        }

        public static coreDto.Contact ToCoreContactDto(this Contact contact)
        {
            return contact.ToContactDto().JsonConvert<coreDto.Contact>();        
        }
    }
}
