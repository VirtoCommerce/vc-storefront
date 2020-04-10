using System;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Catalog.Specifications
{
    public class AggregationItemIsVisibleSpecification : ISpecification<AggregationItem>
    {
        public virtual bool IsSatisfiedBy(AggregationItem obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            var result = obj.Label != null;
            if (obj is CategoryAggregationItem catAggrItem)
            {
                result = !string.IsNullOrEmpty(catAggrItem.CategoryId);
            }
            return result;
        }

    }
}
