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
            var result = new Page
            {
                Author = contentItem.Author,
                Description = contentItem.Description,
                Priority = contentItem.Priority,
                Title = contentItem.Title,
                Type = contentItem.Type,
                Url = contentItem.Url,
                Handle = contentItem.Url,
                Content = contentItem.Content,
                Layout = contentItem.Layout,
                MetaInfo = new MetafieldsCollection("meta_fields", new Dictionary<string, object>())
            };
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