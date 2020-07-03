namespace VirtoCommerce.Storefront.Model.Commands
{
    public class ChangeCartItemQuantityCommand : MutationCommand
    {
        public string LineItemId { get; set; }
        public int Quantity { get; set; }
    }
}
