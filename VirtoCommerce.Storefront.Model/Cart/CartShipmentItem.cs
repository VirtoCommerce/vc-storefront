namespace VirtoCommerce.Storefront.Model.Cart
{
    public partial class CartShipmentItem
    {
        public LineItem LineItem { get; set; }
        public int Quantity { get; set; }
    }
}
