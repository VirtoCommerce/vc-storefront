using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Marketing
{
    /// <summary>
    /// Represents context object for promotion evaluation
    /// </summary>
    public partial class PromotionEvaluationContext : ValueObject
    {
        public PromotionEvaluationContext(Language language, Currency currency)
        {
            Language = language;
            Currency = currency;
        }
            
        public string StoreId { get; set; }
        public Language Language { get; set; }
        public Currency Currency { get; set; } 
        public Customer.CustomerInfo Customer { get; set; }
        public Cart.ShoppingCart Cart { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public Product Product { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StoreId;
            yield return Language;
            yield return Currency;
            yield return Customer;
            yield return Cart;
            yield return Product;
            if(!Products.IsNullOrEmpty())
            {
                foreach(var product in Products)
                {
                    yield return product;
                }
            }
        }

    }
}