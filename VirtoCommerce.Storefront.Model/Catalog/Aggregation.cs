using System;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class Aggregation
    {
        /// <summary>
        /// Aggregation type possible values: range, attr
        /// </summary>
        public string AggregationType { get; set; }
        /// <summary>
        /// Aggregation name: price_usd, color
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// Display name for aggregation
        /// </summary>
        public string Label { get; set; }
        public AggregationItem[] Items { get; set; }

        public virtual void PostLoadInit(AggregationPostLoadContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (Items != null)
            {
                foreach (var aggrItem in Items)
                {
                    aggrItem.PostLoadInit(context);
                }
            }
        }
    }
}
