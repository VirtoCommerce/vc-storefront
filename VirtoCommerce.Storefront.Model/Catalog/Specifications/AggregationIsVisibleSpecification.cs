using System;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Catalog.Specifications
{
    public class AggregationIsVisibleSpecification : ISpecification<Aggregation>
    {
        public virtual bool IsSatisfiedBy(Aggregation obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var result = obj.Items.Any();

            if(result)
            {
                result = obj.Field != null;
            }
            return result;
        }

    }
}
