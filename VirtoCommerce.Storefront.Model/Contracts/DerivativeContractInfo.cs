using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class DerivativeContractInfo : ValueObject
    {
        public string ProductId { get; set; }

        public DerivativeContractType Type { get; set; }

        public decimal ContractSize { get; set; }

        public decimal PurchasedQuantity { get; set; }

        public decimal RemainingQuantity { get; set; }

        #region Overrides of ValueObject

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
        }

        #endregion
    }
}
