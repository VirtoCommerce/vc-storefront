namespace VirtoCommerce.Storefront.Model.Commands
{
    public class ChangeCartItemPriceCommand : MutationCommand
    {
        public string ProductId { get; set; }
        public decimal Price { get; set; }
    }
}
