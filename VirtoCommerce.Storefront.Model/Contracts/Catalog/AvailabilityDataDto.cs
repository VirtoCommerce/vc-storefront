using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class AvailabilityDataDto
    {
        public AvailabilityDataDto()
        {
            Inventories = Array.Empty<InventoryInfoDto>();
        }

        public long AvailableQuantity { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsBuyable { get; set; }

        public bool IsInStock { get; set; }

        public bool IsActive { get; set; }

        public bool IsTrackInventory { get; set; }

        public InventoryInfoDto[] Inventories { get; set; }
    }
}
