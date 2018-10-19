using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model
{
    /// <summary>
    /// Represent menu link  associated to product
    /// </summary>
    public class ProductMenuLink : MenuLink
    {
       public Product Product { get; set; }
    }
}
