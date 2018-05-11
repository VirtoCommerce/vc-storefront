using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Quote
{
    public partial class QuoteSearchCriteria : PagedSearchCriteria
    {
        private static int _defaultPageSize = 20;

        public string CustomerId { get; set; }
        public string Sort { get; set; }
        public string StoreId { get; set; }
        public static int DefaultPageSize
        {
            get { return _defaultPageSize; }
            set { _defaultPageSize = value; }
        }

        public QuoteSearchCriteria()
            : base(new NameValueCollection(), _defaultPageSize)
        {
        }
        public QuoteSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }
    }
}
