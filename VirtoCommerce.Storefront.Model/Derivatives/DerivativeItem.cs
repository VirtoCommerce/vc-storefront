using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Derivatives
{
    public class DerivativeItem : ValueObject
    {
        public string DerivativeId { get; set; }
        public string FulfillmentCenterId { get; set; }
        public string ProductId { get; set; }
        public decimal ContractSize { get; set; }
        public decimal PurchasedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
    }
}
