using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Customer
{
    public class OrganizationContactsSearchCriteria : PagedSearchCriteria
    {

        public static int DefaultPageSize { get; set; } = 20;

        public OrganizationContactsSearchCriteria()
            : base(new Dictionary<string, string>().WithDefaultValue(null), DefaultPageSize)
        {
        }

        public OrganizationContactsSearchCriteria(IDictionary<string, string> queryString)
            : base(queryString, DefaultPageSize)
        {
        }
        public string OrganizationId { get; set; }
        public string Sort { get; set; }
        public string SearchPhrase { get; set; }
    }
}
