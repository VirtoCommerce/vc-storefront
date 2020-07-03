using VirtoCommerce.Storefront.Model.Contracts;

namespace VirtoCommerce.Storefront.Model.Commands
{
    public class AddOrUpdateCartPaymentCommand : MutationCommand
    {
        public PaymentDto Payment { get; set; }
    }
}
