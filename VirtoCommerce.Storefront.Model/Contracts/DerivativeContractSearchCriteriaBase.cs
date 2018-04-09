using System.Collections.Generic;
using System.Collections.Specialized;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public partial class DerivativeContractSearchCriteriaBase : PagedSearchCriteria
    {
        public static int DefaultPageSize { get; set; } = 20;

        public DerivativeContractSearchCriteriaBase()
            : base(new NameValueCollection(), DefaultPageSize)
        {
        }

        public DerivativeContractSearchCriteriaBase(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }

        public IList<DerivativeContractType> Types { get; set; } = new List<DerivativeContractType>();

        public DateTimeRange StartDateRange { get; set; }

        public DateTimeRange EndDateRange { get; set; }

        public bool OnlyActive { get; set; }

        public string SortBy { get; set; }
    }
}
