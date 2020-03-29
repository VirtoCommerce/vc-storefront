using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class AggregationItem : ValueObject
    {
        [JsonIgnore]
        public Aggregation Group { get; set; }
        /// <summary>
        /// Aggregation type possible values: range, attribute
        /// </summary>
        public string GroupType => Group.AggregationType;
        /// <summary>
        /// Display name for aggregation
        /// </summary>
        public string GroupLabel => Group.Label;

        /// <summary>
        /// Total count of items with given value
        /// </summary>
        public int Count { get; set; }
        public bool IsApplied { get; set; }
        public virtual string Label { get; set; }
        public object Value { get; set; }
        public string Lower { get; set; }
        public string Upper { get; set; }

        public bool TermEquals(Term term)
        {
            return Group.Field.EqualsInvariant(term.Name) && Value.ToString().EqualsInvariant(term.Value);
        }
    }
}
