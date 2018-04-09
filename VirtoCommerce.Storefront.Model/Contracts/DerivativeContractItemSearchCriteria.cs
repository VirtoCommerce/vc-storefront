namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class DerivativeContractItemSearchCriteria: DerivativeContractSearchCriteriaBase
    {
        public string DerivativeContractIds { get; set; }

        public string FulfillmentCenterIds { get; set; }

        public string ProductIds { get; set; }
    }
}
