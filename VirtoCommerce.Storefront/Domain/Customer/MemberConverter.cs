﻿using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Stores;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using customerDto = VirtoCommerce.Storefront.AutoRestClients.CustomerModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class CustomerConverterExtension
    {
        public static MemberConverter CustomerConverterInstance => new MemberConverter();

       
        public static Contact ToContact(this customerDto.Contact contactDto)
        {
            return CustomerConverterInstance.ToContact(contactDto);
        }

        public static customerDto.Contact ToCustomerContactDto(this Contact customer)
        {
            return CustomerConverterInstance.ToContactDto(customer);
        }

        public static coreDto.Contact ToCoreContactDto(this Contact customer)
        {
            return CustomerConverterInstance.ToCoreContactDto(customer);
        }

        public static Vendor ToVendor(this customerDto.Vendor vendorDto, Language currentLanguage, Store store)
        {
            return CustomerConverterInstance.ToVendor(vendorDto, currentLanguage, store);
        }

        public static Address ToAddress(this customerDto.Address addressDto)
        {
            return CustomerConverterInstance.ToAddress(addressDto);
        }

        public static customerDto.Address ToCustomerAddressDto(this Address address)
        {
            return CustomerConverterInstance.ToCustomerAddressDto(address);
        }

        public static DynamicProperty ToDynamicProperty(this customerDto.DynamicObjectProperty propertyDto)
        {
            return CustomerConverterInstance.ToDynamicProperty(propertyDto);
        }

        public static customerDto.DynamicObjectProperty ToCustomerDynamicPropertyDto(this DynamicProperty property)
        {
            return CustomerConverterInstance.ToDynamicPropertyDto(property);
        }
    }

    public partial class MemberConverter
    {
        private static readonly char[] _nameSeparator = { ' ' };

        public virtual DynamicProperty ToDynamicProperty(customerDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<coreDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public virtual customerDto.DynamicObjectProperty ToDynamicPropertyDto(DynamicProperty property)
        {
            return property.ToDynamicPropertyDto().JsonConvert<customerDto.DynamicObjectProperty>();
        }

        public virtual Address ToAddress(customerDto.Address addressDto)
        {
            return addressDto.JsonConvert<coreDto.Address>().ToAddress();
        }

        public virtual customerDto.Address ToCustomerAddressDto(Address address)
        {
            return address.ToCoreAddressDto().JsonConvert<customerDto.Address>();
        }

        public virtual Vendor ToVendor(customerDto.Vendor vendorDto, Language currentLanguage, Store store)
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


        public virtual Contact ToContact(customerDto.Contact contactDto)
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
                DefaultLanguage = contactDto.DefaultLanguage
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

            return result;
        }

        
        public virtual customerDto.Contact ToContactDto(Contact customer)
        {
            var retVal = new customerDto.Contact
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                FullName = customer.FullName,
                LastName = customer.LastName,
                MiddleName = customer.MiddleName,
                MemberType = "Contact"
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
           
            return retVal;
        }

        public virtual coreDto.Contact ToCoreContactDto(Contact contact)
        {
            return contact.ToCustomerContactDto().JsonConvert<coreDto.Contact>();        
        }
    }
}
