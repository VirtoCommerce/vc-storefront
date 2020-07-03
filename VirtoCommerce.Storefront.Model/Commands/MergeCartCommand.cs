namespace VirtoCommerce.Storefront.Model.Commands
{
    public class MergeCartCommand : MutationCommand
    {
        public string SecondCartId { get; set; }
    }
}
