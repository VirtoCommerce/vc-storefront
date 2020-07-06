namespace VirtoCommerce.Storefront.Model.Commands
{
    public class ChangeCartItemCommentCommand : MutationCommand
    {
        public string LineItemId { get; set; }
        public string Comment { get; set; }
    }
}
