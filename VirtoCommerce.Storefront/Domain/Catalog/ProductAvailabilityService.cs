using System;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class ProductAvailabilityService : IProductAvailabilityService
    {
        public virtual async Task<bool> IsAvailable(Product product, long requestedQuantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var isAvailable = product.IsActive && product.IsBuyable;

            if (isAvailable && product.TrackInventory && product.Inventory != null)
            {
                isAvailable = product.Inventory.AllowPreorder == true ||
                              product.Inventory.AllowBackorder == true ||
                              await GetAvailableQuantity(product) >= requestedQuantity;
            }

            return isAvailable;
        }

        public bool IsBuyable(Product product)
         {
             if (product == null)
                 throw new ArgumentNullException(nameof(product));
 
             var isBuyable = product.IsActive && product.IsBuyable;
 
             return  isBuyable;
         }
 
         public virtual async Task<bool> IsInStock(Product product)
         {
             if (product == null)
                 throw new ArgumentNullException(nameof(product));
 
             bool inStock = false;
             if (product.TrackInventory && product.Inventory != null)
             {
                     inStock = product.Inventory.AllowPreorder == true ||
                               product.Inventory.AllowBackorder == true ||
                              await GetAvailableQuantity(product) > 0;
             }
 
             return inStock;
         }

        public virtual Task<long> GetAvailableQuantity(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            long availableQuantity = 0;

            if (product.TrackInventory && product.InventoryAll != null)
            {
                foreach (var inventory in product.InventoryAll)
                {
                    availableQuantity += Math.Max(0, (inventory.InStockQuantity ?? 0L) - (inventory.ReservedQuantity ?? 0L));
                }
            }

            return Task.FromResult(availableQuantity);
        }
    }
}