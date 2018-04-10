using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public partial class DerivativeContractItem : ValueObject
    {
        public string DerivativeContractId { get; set; }

        public string FulfillmentCenterId { get; set; }

        public string ProductId { get; set; }

        public long ContractSize { get; set; }

        public long PurchasedQuantity { get; set; }

        public long RemainingQuantity { get; set; }
    }
}
