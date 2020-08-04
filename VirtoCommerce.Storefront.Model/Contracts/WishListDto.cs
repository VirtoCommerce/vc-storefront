namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class WishListDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StoreId { get; set; }
        public string CustomerId { get; set; }
        public string Currency { get; set; }
        public string LanguageCode { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
    }
}
