using System;
using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class ProductIsInStockSpecification : ISpecification<Product>
    {
        public virtual bool IsSatisfiedBy(Product product)
        {
            var result = true;
            if (product.TrackInventory)
            {
                result = product.Inventory != null &&
                    product.AvailableQuantity +
                    (Convert.ToInt32(product.Inventory.AllowPreorder) * Convert.ToInt32(product.Inventory.PreorderQuantity)) +
                    (Convert.ToInt32(product.Inventory.AllowBackorder) * Convert.ToInt32(product.Inventory.BackorderQuantity))
                    > 0;
            }
            return result;
        }

    }
}
