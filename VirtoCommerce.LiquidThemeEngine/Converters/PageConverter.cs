using System.Collections.Generic;
using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class PageConverter
    {
        public static Page ToShopifyModel(this StorefrontModel.StaticContent.ContentItem contentItem)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidPage(contentItem);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Page ToLiquidPage(StorefrontModel.StaticContent.ContentItem contentItem)
        {
            var result = new Page();
            result.Author = contentItem.Author;
            result.Description = contentItem.Description;
            result.Priority = contentItem.Priority;
            result.Title = contentItem.Title;
            result.Type = contentItem.Type;
            result.Url = contentItem.Url;
            result.Handle = contentItem.Url;

            result.MetaInfo = new MetafieldsCollection("meta_fields", new Dictionary<string, object>());
            foreach (var metaInfoProp in contentItem.MetaInfo)
            {
                result.MetaInfo.Add(metaInfoProp.Key, metaInfoProp.Value);
            }
            
            if (contentItem.PublishedDate.HasValue)
            {
                result.PublishedAt = contentItem.PublishedDate.Value;
            }
            return result;
        }
    }
}