using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Tax
{
    public partial class TaxEvaluationContext : ValueObject
    {
        public TaxEvaluationContext(string storeId)
        {
            StoreId = storeId;
            Lines = new List<TaxLine>();
            StoreTaxCalculationEnabled = true;
        }
        //It not a context identifier
        public string Id { get; set; }
        public string StoreId { get; set; }

        public string Code { get; set; }

         public string Type { get; set; }

        public Customer.CustomerInfo Customer { get; set; }

        public Address Address { get; set; }

        public Currency Currency { get; set; }

        public ICollection<TaxLine> Lines { get; set; }

        public bool StoreTaxCalculationEnabled { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return StoreId;
            yield return Code;
            yield return Type;
            yield return Customer;
            yield return Address;
            yield return Currency;
            yield return StoreTaxCalculationEnabled;
            if(Lines != null)
            {
                foreach(var line in Lines)
                {
                    yield return line;
                }
            }
        }

    }
}
