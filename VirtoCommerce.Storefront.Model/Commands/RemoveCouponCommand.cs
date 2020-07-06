namespace VirtoCommerce.Storefront.Model.Commands
{
    public class RemoveCouponCommand : MutationCommand
    {
        public string CouponCode { get; set; }
    }
}
