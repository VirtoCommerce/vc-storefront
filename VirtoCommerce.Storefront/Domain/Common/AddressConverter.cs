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
            var retVal = new Address();
            retVal.City = addressDto.City;
            retVal.CountryCode = addressDto.CountryCode;
            retVal.CountryName = addressDto.CountryName;
            retVal.Email = addressDto.Email;
            retVal.FirstName = addressDto.FirstName;
            retVal.LastName = addressDto.LastName;
            retVal.Line1 = addressDto.Line1;
            retVal.Line2 = addressDto.Line2;
            retVal.MiddleName = addressDto.MiddleName;
            retVal.Name = addressDto.Name;
            retVal.Organization = addressDto.Organization;
            retVal.Phone = addressDto.Phone;
            retVal.PostalCode = addressDto.PostalCode;
            retVal.RegionId = addressDto.RegionId;
            retVal.RegionName = addressDto.RegionName;
            retVal.Zip = addressDto.Zip;
           
            retVal.Type = (AddressType)Enum.Parse(typeof(AddressType), addressDto.AddressType, true);
            return retVal;
        }

        public static coreDto.Address ToCoreAddressDto(this Address address)
        {
            var retVal = new coreDto.Address();

            retVal.City = address.City;
            retVal.CountryCode = address.CountryCode;
            retVal.CountryName = address.CountryName ?? address.CountryCode;
            retVal.Email = address.Email;
            retVal.FirstName = address.FirstName;
            retVal.LastName = address.LastName;
            retVal.Line1 = address.Line1;
            retVal.Line2 = address.Line2;
            retVal.MiddleName = address.MiddleName;
            retVal.Name = address.Name ?? address.Line1;
            retVal.Organization = address.Organization;
            retVal.Phone = address.Phone;
            retVal.PostalCode = address.PostalCode;
            retVal.RegionId = address.RegionId;
            retVal.RegionName = address.RegionName;
            retVal.Zip = address.Zip;

            retVal.AddressType = address.Type.ToString();

            return retVal;
        }


        public static Address ToWebModel(this shopifyModel.Address address, Country[] countries)
        {
            var result = new Address();
            result.CopyFrom(address, countries);
            return result;
        }


        public static Address CopyFrom(this Address result, shopifyModel.Address address, Country[] countries)
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
