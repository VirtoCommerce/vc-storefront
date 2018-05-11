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

            if (result && _product.TrackInventory && _product.Inventory != null)
            {
                result = _product.Inventory.AllowPreorder == true ||
                              _product.Inventory.AllowBackorder == true ||
                              _product.AvailableQuantity >= requestedQuantity;
            }

            return result;
        }
        
    }
}
