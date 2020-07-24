using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class InventoryInfoDto
    {
        public bool? AllowBackorder { get; set; }

        public bool? AllowPreorder { get; set; }

        public DateTime? BackorderAvailableDate { get; set; }

        public string FulfillmentCenterName { get; set; }

        public string FulfillmentCenterId { get; set; }

        public long? InStockQuantity { get; set; }

        public DateTime? PreorderAvailabilityDate { get; set; }
    }
}
