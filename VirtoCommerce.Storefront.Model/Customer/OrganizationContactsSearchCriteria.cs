using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Customer
{
    public class OrganizationContactsSearchCriteria : PagedSearchCriteria
    {
        private static int _defaultPageSize = 20;

        public static int DefaultPageSize
        {
            get { return _defaultPageSize; }
            set { _defaultPageSize = value; }
        }

        public OrganizationContactsSearchCriteria()
            : base(new NameValueCollection(), _defaultPageSize)
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
