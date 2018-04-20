using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Inventory
{
    public partial class FulfillmentCenterSearchCriteria : PagedSearchCriteria
    {
        public static int DefaultPageSize { get; set; } = 20;

        public FulfillmentCenterSearchCriteria()
            : base(new NameValueCollection(), DefaultPageSize)
        {
        }

        public FulfillmentCenterSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }
       
        public string Sort { get; set; }
        public string SearchPhrase { get; set; }
    }
}
