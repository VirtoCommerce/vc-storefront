namespace VirtoCommerce.Storefront.Model.Commands
{
    public class RemoveCartItemCommand : MutationCommand
    {
        public string LineItemId { get; set; }
    }
}
