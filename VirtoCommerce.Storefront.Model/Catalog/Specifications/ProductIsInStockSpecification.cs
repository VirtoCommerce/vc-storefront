using System;
using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class ProductIsInStockSpecification : ISpecification<Product>
    {
        public virtual bool IsSatisfiedBy(Product product)
        {
            var result = true;
            if (product.TrackInventory && product.Inventory != null)
            {
                result = product.AvailableQuantity +
                    (Convert.ToInt32(product.Inventory.AllowPreorder) * product.Inventory.PreorderQuantity) +
                    (Convert.ToInt32(product.Inventory.AllowBackorder) * product.Inventory.BackorderQuantity)
                    > 0;

            }
            return result;
        }

    }
}
