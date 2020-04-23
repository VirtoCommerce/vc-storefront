using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public class CategoryAggregationItem : AggregationItem
    {
        public string CatalogId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
        public string CategoryId { get; set; }

        public override string Label
        {
            get => Category != null ? Category.Title : base.Label;
            set => base.Label = value;
        }
        public static CategoryAggregationItem FromAggregationItem(AggregationItem aggregationItem)
        {
            if (aggregationItem == null)
            {
                throw new ArgumentNullException(nameof(aggregationItem));
            }
            var result = new CategoryAggregationItem
            {
                Group = aggregationItem.Group,
                Value = aggregationItem.Value,
                Count = aggregationItem.Count,
                Label = aggregationItem.Label
            };
            var parts = aggregationItem.Value?.ToString().Split('/');
            if (parts != null)
            {
                if (parts.Length > 0)
                {
                    result.CatalogId = parts[0];
                }
                if (parts.Length > 1)
                {
                    result.CategoryId = parts[parts.Length - 1];
                }
            }
            return result;
        }


        public override void PostLoadInit(AggregationPostLoadContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            base.PostLoadInit(context);

            if (!string.IsNullOrEmpty(CategoryId) && Category == null)
            {
                Category = context.CategoryByIdDict[CategoryId];
            }

        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CatalogId;
            yield return CategoryId;
        }
    }
}
