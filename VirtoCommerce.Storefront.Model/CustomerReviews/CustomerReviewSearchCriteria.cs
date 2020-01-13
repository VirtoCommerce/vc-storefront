using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.CustomerReviews
{
    public partial class CustomerReviewSearchCriteria : PagedSearchCriteria
    {
        public static int DefaultPageSize { get; set; } = 20;

        public CustomerReviewSearchCriteria()
            : base(new NameValueCollection(), DefaultPageSize)
        {
        }
        public CustomerReviewSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }

        public string[] ProductIds { get; set; }
        public bool? IsActive { get; set; }
        public string Sort { get; set; }
    }
}
