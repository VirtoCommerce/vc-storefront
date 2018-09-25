using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class JsonPageConverter
    {
        public static JsonPage ToShopifyModel(this StorefrontModel.JsonPage page)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidCmsPage(page);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual JsonPage ToLiquidCmsPage(StorefrontModel.JsonPage source)
        {
            var result = new JsonPage
            {
                Settings = source.Settings,
                Blocks = new List<IDictionary<string, object>>()
            };

            foreach (var block in source.Blocks)
            {
                var collection = new MetafieldsCollection(string.Empty, block);
                result.Blocks.Add(collection);
            }

            return result;
        }
    }
}
