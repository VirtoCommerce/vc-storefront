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
                result = product.Inventory.AllowPreorder == true || product.Inventory.AllowBackorder == true || product.AvailableQuantity > 0;
            }
            return result;
        }
        
    }
}
