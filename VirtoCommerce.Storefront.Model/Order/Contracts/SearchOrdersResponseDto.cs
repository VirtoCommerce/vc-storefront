namespace VirtoCommerce.Storefront.Model.Order.Contracts
{
    public class SearchOrdersResponseDto
    {
        public int TotalCount { get; set; }
        public CustomerOrder[] Items { get; set; }
    }
}
