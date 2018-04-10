using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class AddressConverter
    {
        public static Address ToShopifyModel(this StorefrontModel.Address address)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidAddress(address);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Address ToLiquidAddress(StorefrontModel.Address address)
        {
            Address result = null;

            if (address != null)
            {
                result = new Address();
                result.City = address.City;
                result.FirstName = address.FirstName;
                result.LastName = address.LastName;
                result.Name = address.Name;
                result.Phone = address.Phone;
                result.Address1 = address.Line1;
                result.Address2 = address.Line2;
                result.Street = string.Join(", ", result.Address1, result.Address2).Trim(',', ' ');
                result.Company = address.Organization;
                result.Province = address.RegionName;
                result.ProvinceCode = address.RegionId;
                result.Zip = address.PostalCode;
                result.Country = address.CountryName;
                result.CountryCode = address.CountryCode;
                result.Id = address.GetHashCode().ToString();
            }

            return result;
        }
    }
}
