using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class TagConverter
    {
        public static Tag ToShopifyModel(this Term term)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidTag(term);
        }

        public static Tag ToShopifyModel(this AggregationItem item, Aggregation aggregation)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidTag(item, aggregation);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Tag ToLiquidTag(Term term)
        {
            var result = new Tag(term.Name, term.Value);
            return result;
        }

        public virtual Tag ToLiquidTag(AggregationItem item, Aggregation aggregation)
        {
            var result = new Tag(aggregation.Field, item.Value?.ToString())
            {
                GroupType = aggregation.AggregationType,
                GroupLabel = aggregation.Label,
                Label = item.Label,
                Count = item.Count,
                Lower = item.Lower,
                Upper = item.Upper,
            };

            return result;
        }
    }
}
