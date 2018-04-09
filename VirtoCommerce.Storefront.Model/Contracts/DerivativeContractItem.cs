using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public partial class DerivativeContractItem : ValueObject
    {
        public string DerivativeContractId { get; set; }

        public string FulfillmentCenterId { get; set; }

        public string ProductId { get; set; }

        public decimal ContractSize { get; set; }

        public decimal PurchasedQuantity { get; set; }

        public decimal RemainingQuantity { get; set; }
    }
}
