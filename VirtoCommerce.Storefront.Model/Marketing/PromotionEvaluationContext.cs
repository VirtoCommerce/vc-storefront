using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Marketing
{
    /// <summary>
    /// Represents context object for promotion evaluation
    /// </summary>
    public partial class PromotionEvaluationContext
    {
        public PromotionEvaluationContext()
        {     
        }
            
        public string StoreId { get; set; }
        public Language Language { get; set; }
        public Currency Currency { get; set; } 
        public Customer.CustomerInfo Customer { get; set; }
        public Cart.ShoppingCart Cart { get; set; }
        public ICollection<Product> Products { get; set; }
        public Product Product { get; set; }
    
    }
}