namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class AddressDto
    {
        //[JsonProperty(PropertyName = "city")]
        public string City { get; set; }
        //[JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }
        //[JsonProperty(PropertyName = "countryName")]
        public string CountryName { get; set; }
        //[JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        //[JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }
        //[JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
        //[JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
        //[JsonProperty(PropertyName = "line1")]
        public string Line1 { get; set; }
        //[JsonProperty(PropertyName = "line2")]
        public string Line2 { get; set; }
        //[JsonProperty(PropertyName = "middleName")]
        public string MiddleName { get; set; }
        //[JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        //[JsonProperty(PropertyName = "organization")]
        public string Organization { get; set; }
        //[JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }
        //[JsonProperty(PropertyName = "postalCode")]
        public string PostalCode { get; set; }
        //[JsonProperty(PropertyName = "regionId")]
        public string RegionId { get; set; }
        //[JsonProperty(PropertyName = "regionName")]
        public string RegionName { get; set; }
        //[JsonProperty(PropertyName = "zip")]
        public string Zip { get; set; }
    }
}
