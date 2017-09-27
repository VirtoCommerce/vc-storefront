using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using inventoryDto = VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class InventoryConverter
    {
        public static Inventory ToInventory(this inventoryDto.InventoryInfo inventoryDto)
        {
            var result = new Inventory();
            result.AllowBackorder = inventoryDto.AllowBackorder;
            result.AllowPreorder = inventoryDto.AllowPreorder;
            result.BackorderAvailabilityDate = inventoryDto.BackorderAvailabilityDate;
            result.FulfillmentCenterId = inventoryDto.FulfillmentCenterId;
            result.InStockQuantity = inventoryDto.InStockQuantity;
            result.PreorderAvailabilityDate = inventoryDto.PreorderAvailabilityDate;
            result.ProductId = inventoryDto.ProductId;
            result.ReservedQuantity = inventoryDto.ReservedQuantity;
           
            result.Status = EnumUtility.SafeParse(inventoryDto.Status, InventoryStatus.Disabled);

            return result;
        }
    }
}
