using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Quote;

namespace VirtoCommerce.Storefront.Model.Common.SearchResults
{
    public class QuoteSearchResult
    {
        public QuoteSearchResult()
        {
            Results = new List<QuoteRequest>();
        }

        public int TotalCount { get; set; }
        public ICollection<QuoteRequest> Results { get; set; }
    }
}
