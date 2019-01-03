using System.Collections.Generic;
using System.Linq;
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
            };
            if (contentItem.MetaInfo != null)
            {
                result.MetaInfo = new Dictionary<string, IDictionary<string, object>>
                {
                    ["meta_fields"] = contentItem.MetaInfo.ToDictionary(prop => prop.Key, prop =>
                    {
                        return (object)prop.Value;
                    })
                };
            }
            if (contentItem.PublishedDate.HasValue)
            {
                result.PublishedAt = contentItem.PublishedDate.Value;
            }
            return result;
        }
    }
}
