using System;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using shopifyModel = VirtoCommerce.LiquidThemeEngine.Objects;

namespace VirtoCommerce.Storefront.Domain
{
    public static class AddressConverter
    {
        public static Address ToAddress(this coreDto.Address addressDto)
        {
            var retVal = new Address
            {
                Key = addressDto.Key,
                City = addressDto.City,
                CountryCode = addressDto.CountryCode,
                CountryName = addressDto.CountryName,
                Email = addressDto.Email,
                FirstName = addressDto.FirstName,
                LastName = addressDto.LastName,
                Line1 = addressDto.Line1,
                Line2 = addressDto.Line2,
                MiddleName = addressDto.MiddleName,
                Name = addressDto.Name,
                Organization = addressDto.Organization,
                Phone = addressDto.Phone,
                PostalCode = addressDto.PostalCode,
                RegionId = addressDto.RegionId,
                RegionName = addressDto.RegionName,
                Zip = addressDto.Zip,

                Type = (AddressType)Enum.Parse(typeof(AddressType), addressDto.AddressType, true)
            };
            return retVal;
        }

        public static coreDto.Address ToCoreAddressDto(this Address address)
        {
            var retVal = new coreDto.Address
            {
                Key = address.Key,
                City = address.City,
                CountryCode = address.CountryCode,
                CountryName = address.CountryName,
                Email = address.Email,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Line1 = address.Line1,
                Line2 = address.Line2,
                MiddleName = address.MiddleName,
                Name = address.Name,
                Organization = address.Organization,
                Phone = address.Phone,
                PostalCode = address.PostalCode,
                RegionId = address.RegionId,
                RegionName = address.RegionName,
                Zip = address.Zip,

                AddressType = address.Type.ToString()
            };

            return retVal;
        }


        public static Address ToWebModel(this Address address, Country[] countries)
        {
            var result = new Address();
            result.CopyFrom(address, countries);
            return result;
        }


        public static Address CopyFrom(this Address result, Address address, Country[] countries)
        {
            result.City = address.City;
            result.CountryCode = address.CountryCode;
            result.FirstName = address.FirstName;
            result.LastName = address.LastName;
            result.Name = address.Name;
            result.Phone = address.Phone;
            result.Zip = address.Zip;
            result.Organization = address.Company;
            result.CountryName = address.Country;
            result.PostalCode = address.Zip;
            result.Line1 = address.Address1;
            result.Line2 = address.Address2;
            result.RegionName = address.Province;

            result.Name = result.ToString();

            var country = countries.FirstOrDefault(c => string.Equals(c.Name, address.Country, StringComparison.OrdinalIgnoreCase));
            if (country != null)
            {
                result.CountryCode = country.Code3;

                if (address.Province != null && country.Regions != null)
                {
                    var region = country.Regions.FirstOrDefault(r => string.Equals(r.Name, address.Province, StringComparison.OrdinalIgnoreCase));

                    if (region != null)
                    {
                        result.RegionId = region.Code;
                    }
                }
            }

            return result;
        }
    }
}
