using System;
using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Contracts
{
    public partial class DerivativeContract : Entity
    {
        public string MemberId { get; set; }

        public DerivativeContractType Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        public IList<DerivativeContractItem> Items { get; set; }
    }
}
