namespace VirtoCommerce.Storefront.Model.Commands
{
    public class ChangeCartItemQuantityCommand : MutationCommand
    {
        public string ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
