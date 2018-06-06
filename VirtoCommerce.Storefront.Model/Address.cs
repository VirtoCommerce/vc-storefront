using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class Address : ValueObject
    {
        public AddressType Type { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Zip { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string RegionId { get; set; }
        public string RegionName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        [Phone]
        public string Phone { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        public override string ToString()
        {
            var retVal = string.Join(" ", FirstName, LastName, Organization, Line1, City, RegionName, PostalCode, CountryName);
            return retVal;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            //Key and Name properties don't participate in equality
            yield return Type;
            yield return Organization;
            yield return CountryCode;
            yield return CountryName;
            yield return PostalCode;
            yield return Zip;
            yield return Line1;
            yield return Line2;
            yield return RegionId;
            yield return RegionName;
            yield return FirstName;
            yield return MiddleName;
            yield return LastName;
            yield return Phone;
            yield return Email;
        }
    }
}
