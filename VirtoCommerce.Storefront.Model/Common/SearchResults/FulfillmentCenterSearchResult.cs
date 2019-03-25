using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Inventory;

namespace VirtoCommerce.Storefront.Model.Common.SearchResults
{
    public class FulfillmentCenterSearchResult
    {
        public FulfillmentCenterSearchResult()
        {
            Results = new List<FulfillmentCenter>();
        }

        public int TotalCount { get; set; }
        public ICollection<FulfillmentCenter> Results { get; set; }
    }
}
