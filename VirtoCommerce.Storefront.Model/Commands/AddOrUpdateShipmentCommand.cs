using VirtoCommerce.Storefront.Model.Contracts;

namespace VirtoCommerce.Storefront.Model.Commands
{
    public class AddOrUpdateShipmentCommand : MutationCommand
    {
        public ShipmentDto Shipment { get; set; }
    }
}
