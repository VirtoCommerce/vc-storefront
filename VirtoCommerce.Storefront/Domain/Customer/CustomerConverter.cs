using System;
using System.Linq;
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
        public static CustomerConverter CustomerConverterInstance => new CustomerConverter();

        public static CustomerInfo ToCustomerInfo(this Register formModel)
        {
            return CustomerConverterInstance.ToCustomerInfo(formModel);
        }

        public static CustomerInfo ToCustomerInfo(this customerDto.Member memberDto)
        {
            return CustomerConverterInstance.ToCustomerInfo(memberDto);
        }
        
      
        public static customerDto.Contact ToCustomerContactDto(this CustomerInfo customer)
        {
            return CustomerConverterInstance.ToCustomerContactDto(customer);
        }

        public static coreDto.Contact ToCoreContactDto(this CustomerInfo customer)
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
            return CustomerConverterInstance.ToCustomerDynamicPropertyDto(property);
        }
    }

    public partial class CustomerConverter
    {
        private static readonly char[] _nameSeparator = { ' ' };


        public virtual DynamicProperty ToDynamicProperty(customerDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<coreDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public virtual customerDto.DynamicObjectProperty ToCustomerDynamicPropertyDto(DynamicProperty property)
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
                result = new Vendor();
                result.Id = vendorDto.Id;
                result.Name = vendorDto.Name;
                result.Description = vendorDto.Description;
                result.LogoUrl = vendorDto.LogoUrl;
                result.SiteUrl = vendorDto.SiteUrl;
                result.GroupName = vendorDto.GroupName;

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

        public virtual CustomerInfo ToCustomerInfo(Register formModel)
        {
            var result = new CustomerInfo
            {
                Email = formModel.Email,
                UserName = formModel.UserName,
                FullName = string.Join(" ", formModel.FirstName, formModel.LastName),
                FirstName = formModel.FirstName,
                LastName = formModel.LastName
            };

            if (string.IsNullOrEmpty(result.FullName) || string.IsNullOrWhiteSpace(result.FullName))
            {
                result.FullName = formModel.Email;
            }
            return result;
        }

        public virtual CustomerInfo ToCustomerInfo(customerDto.Member memberDto)
        {
            var result = new CustomerInfo();
            result.Id = memberDto.Id;
         

            result.IsRegisteredUser = true;
            result.MemberType = memberDto.MemberType;
            result.UserGroups = memberDto.Groups;
            if (memberDto.Addresses != null)
            {
                result.Addresses = memberDto.Addresses.Select(ToAddress).ToList();
            }

            result.DefaultBillingAddress = result.Addresses.FirstOrDefault(a => (a.Type & AddressType.Billing) == AddressType.Billing);
            result.DefaultShippingAddress = result.Addresses.FirstOrDefault(a => (a.Type & AddressType.Shipping) == AddressType.Shipping);
            
            if (!string.IsNullOrEmpty(memberDto.Name))
            {
                result.FullName = memberDto.Name;

                var nameParts = memberDto.Name.Split(_nameSeparator, 2);

                if (nameParts.Length > 0)
                {
                    result.FirstName = nameParts[0];
                }

                if (nameParts.Length > 1)
                {
                    result.LastName = nameParts[1];
                }
            }

            if (memberDto.Emails != null)
            {
                result.Email = memberDto.Emails.FirstOrDefault();
            }
            if (!memberDto.DynamicProperties.IsNullOrEmpty())
            {
                result.DynamicProperties = memberDto.DynamicProperties.Select(ToDynamicProperty).ToList();
            }

            return result;
        }

        
        public virtual customerDto.Contact ToCustomerContactDto(CustomerInfo customer)
        {
            var retVal = new customerDto.Contact();
            retVal.Id = customer.Id;
            retVal.FirstName = customer.FirstName;
            retVal.FullName = customer.FullName;
            retVal.LastName = customer.LastName;
            retVal.MiddleName = customer.MiddleName;
            retVal.MemberType = customer.MemberType;
            if (customer.UserGroups != null)
            {
                retVal.Groups = customer.UserGroups.ToArray();
            }
            if (customer.Addresses != null)
            {
                retVal.Addresses = customer.Addresses.Select(ToCustomerAddressDto).ToList();
            }
            if (!string.IsNullOrEmpty(customer.Email))
            {
                retVal.Emails = new[] { customer.Email }.ToList();
            }
            retVal.FullName = customer.FullName;

            return retVal;
        }

        public virtual coreDto.Contact ToCoreContactDto(CustomerInfo customer)
        {
            var retVal = new coreDto.Contact();

            retVal.Id = customer.Id;
            retVal.FirstName = customer.FirstName;
            retVal.LastName = customer.LastName;
            retVal.MiddleName = customer.MiddleName;
            retVal.MemberType = customer.MemberType;

            if (customer.UserGroups != null)
            {
                retVal.Groups = customer.UserGroups.ToArray();
            }
            if (customer.Addresses != null)
            {
                retVal.Addresses = customer.Addresses.Select(x => x.ToCoreAddressDto()).ToList();
            }
            if (!string.IsNullOrEmpty(customer.Email))
            {
                retVal.Emails = new[] { customer.Email }.ToList();
            }
            retVal.FullName = customer.FullName;

            return retVal;
        }
    }
}
