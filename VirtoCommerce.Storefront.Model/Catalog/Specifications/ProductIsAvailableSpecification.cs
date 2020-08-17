using System;
using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class ProductIsAvailableSpecification : ISpecification<long>
    {
        private readonly Product _product;
        public ProductIsAvailableSpecification(Product product)
        {
            _product = product;
        }

        public virtual bool IsSatisfiedBy(long requestedQuantity)
        {
            var result = new ProductIsBuyableSpecification().IsSatisfiedBy(_product);

            if (result && _product.TrackInventory)
            {
                result = _product.Inventory != null &&
                         _product.AvailableQuantity +
                         (Convert.ToInt32(_product.Inventory.AllowPreorder) * Convert.ToInt32(_product.Inventory.PreorderQuantity)) +
                         (Convert.ToInt32(_product.Inventory.AllowBackorder) * Convert.ToInt32(_product.Inventory.BackorderQuantity))
                         >= requestedQuantity;
            }

            return result;
        }

    }
}
