using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Tax
{
    public partial class TaxEvaluationContext
    {
        public TaxEvaluationContext(string storeId)
        {
            StoreId = storeId;
            Lines = new List<TaxLine>();
            StoreTaxCalculationEnabled = true;
        }
        public string StoreId { get; set; }

        public string Code { get; set; }

         public string Type { get; set; }

        public Customer.CustomerInfo Customer { get; set; }

        public Address Address { get; set; }

        public Currency Currency { get; set; }

        public ICollection<TaxLine> Lines { get; set; }

        public string Id { get; set; }

        public bool StoreTaxCalculationEnabled { get; set; }
    }
}
