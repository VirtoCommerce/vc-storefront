using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Cart;

namespace VirtoCommerce.Storefront.Model.Common.SearchResults
{
    public class ShoppingCartSearchResult
    {
        public ShoppingCartSearchResult()
        {
            Results = new List<ShoppingCart>();
        }

        public int TotalCount { get; set; }
        public ICollection<ShoppingCart> Results { get; set; }

    }
}
