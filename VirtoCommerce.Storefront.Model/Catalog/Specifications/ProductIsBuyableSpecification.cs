using System;
using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class ProductIsBuyableSpecification : ISpecification<Product>
    {
        public virtual bool IsSatisfiedBy(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            return product.IsActive && product.IsBuyable && product.Price.ListPrice.Amount > 0;
        }

    }
}
