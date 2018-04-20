using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Inventory;
using inventoryDto = VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class InventoryConverter
    {
        public static Inventory ToInventory(this inventoryDto.InventoryInfo inventoryDto)
        {
            var result = new Inventory
            {
                AllowBackorder = inventoryDto.AllowBackorder,
                AllowPreorder = inventoryDto.AllowPreorder,
                BackorderAvailabilityDate = inventoryDto.BackorderAvailabilityDate,
                FulfillmentCenterId = inventoryDto.FulfillmentCenterId,
                InStockQuantity = inventoryDto.InStockQuantity,
                PreorderAvailabilityDate = inventoryDto.PreorderAvailabilityDate,
                ProductId = inventoryDto.ProductId,
                ReservedQuantity = inventoryDto.ReservedQuantity,

                Status = EnumUtility.SafeParse(inventoryDto.Status, InventoryStatus.Disabled)
            };

            return result;
        }

        public static FulfillmentCenter ToFulfillmentCenter(this inventoryDto.FulfillmentCenter centerDto)
        {
            var result = new FulfillmentCenter
            {
                Name = centerDto.Name,
                Description = centerDto.Description,
                GeoLocation = centerDto.GeoLocation,
                Id = centerDto.Id
            };
           if(centerDto.Address != null)
            {
                result.Address = centerDto.Address.JsonConvert<AutoRestClients.CoreModuleApi.Models.Address>().ToAddress();
            }
            return result;
        }


    }
}
