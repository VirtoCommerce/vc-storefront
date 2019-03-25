using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Subscriptions;

namespace VirtoCommerce.Storefront.Model.Common.SearchResults
{
    public class SubscriptionSearchResult
    {
        public SubscriptionSearchResult()
        {
            Results = new List<Subscription>();
        }

        public int TotalCount { get; set; }
        public ICollection<Subscription> Results { get; set; }
    }
}
