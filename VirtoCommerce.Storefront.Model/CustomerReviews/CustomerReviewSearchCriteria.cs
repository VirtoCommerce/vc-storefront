using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.CustomerReviews
{
    public class CustomerReviewSearchCriteria : PagedSearchCriteria
    {
        private static int _defaultPageSize = 20;

        public static int DefaultPageSize
        {
            get => _defaultPageSize;
            set => _defaultPageSize = value;
        }

        public CustomerReviewSearchCriteria()
            : base(new NameValueCollection(), _defaultPageSize)
        {
        }

        public CustomerReviewSearchCriteria(NameValueCollection queryString, int defaultPageSize)
            : base(queryString, defaultPageSize)
        {
        }

        public string[] ProductIds { get; set; }
        public bool? IsActive { get; set; }
        public string Sort { get; set; }
    }
}
