using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public class DerivativeContractInfo : ValueObject
    {
        public string ProductId { get; set; }

        public DerivativeContractType Type { get; set; }

        public long ContractSize { get; set; }

        public long PurchasedQuantity { get; set; }

        public long RemainingQuantity { get; set; }

        #region Overrides of ValueObject

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProductId;
            yield return Type;
        }

        #endregion
    }
}
