namespace VirtoCommerce.Storefront.Model.Commands
{
    public class RemoveShipmentCommand : MutationCommand
    {
        public string ShipmentId { get; set; }
    }
}
