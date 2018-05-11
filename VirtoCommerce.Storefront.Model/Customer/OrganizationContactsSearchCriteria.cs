using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Customer
{
    public class OrganizationContactsSearchCriteria : PagedSearchCriteria
    {

        public static int DefaultPageSize { get; set; } = 20;

        public OrganizationContactsSearchCriteria()
            : base(new NameValueCollection(), DefaultPageSize)
        {
        }

        public OrganizationContactsSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }
        public string OrganizationId { get; set; }
        public string Sort { get; set; }
        public string SearchPhrase { get; set; }
    }
}
