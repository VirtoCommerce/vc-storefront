namespace VirtoCommerce.Storefront.Model.Cart
{
    public class CartShipmentItemDto
    {
        public int Quantity { get; set; }
        public LineItemDto LineItem { get; set; }
    }
}
