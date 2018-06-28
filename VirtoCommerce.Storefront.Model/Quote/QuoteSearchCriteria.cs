using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Quote
{
    public partial class QuoteSearchCriteria : PagedSearchCriteria
    {
        public string CustomerId { get; set; }
        public string Sort { get; set; }
        public string StoreId { get; set; }
        public static int DefaultPageSize { get; set; } = 20;

        public QuoteSearchCriteria()
            : base(new NameValueCollection(), DefaultPageSize)
        {
        }
        public QuoteSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }
    }
}
