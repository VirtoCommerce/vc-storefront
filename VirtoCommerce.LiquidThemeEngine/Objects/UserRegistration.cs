using System.Runtime.Serialization;
using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    [DataContract]
    public class UserRegistration : Drop
    {
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string StoreId { get; set; }
        [DataMember]
        public string Role { get; set; }
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public string OrganizationId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Address Address { get; set; }
    }
}
