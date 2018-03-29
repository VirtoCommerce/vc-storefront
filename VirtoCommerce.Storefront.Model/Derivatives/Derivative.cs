using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Derivatives
{
    public class Derivative : Entity
    {
        public string MemberId { get; set; }
        public DerivativeType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<DerivativeItem> Items { get; set; }
    }
}
