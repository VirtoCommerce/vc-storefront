using System.Runtime.Serialization;

namespace VirtoCommerce.Storefront.Model.Cart
{
    [DataContract(Name = "context")]
    public class ShoppingCartContextDto
    {
        [DataMember(Name = "storeId")]
        public string StoreId { get; set; }
        [DataMember(Name = "cartName")]
        public string CartName { get; set; }
        [DataMember(Name = "userId")]
        public string UserId { get; set; }
        [DataMember(Name = "cultureName")]
        public string CultureName { get; set; }
        [DataMember(Name = "currencyCode")]
        public string CurrencyCode { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
