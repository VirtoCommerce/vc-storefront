using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class ProductBreadcrumb : Breadcrumb
    {
        public ProductBreadcrumb(Product product) : base(nameof(Product))
        {
            Product = product;
        }
        public Product Product { get; private set; }
    }
}
