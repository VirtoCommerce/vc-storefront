namespace VirtoCommerce.Storefront.Model.Cart
{
    public class ChangeCartItemPrice
    {
        public string LineItemId { get; set; }
        public decimal NewPrice { get; set; }
    }
}
