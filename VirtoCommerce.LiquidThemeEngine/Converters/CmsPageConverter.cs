using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Cms;
using VirtoCommerce.Storefront.Model.Common;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class CmsPageConverter
    {
        public static CmsPage ToShopifyModel(this StorefrontModel.Cms.CmsPageDefinition cmsPage)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidCmsPage(cmsPage);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual CmsPage ToLiquidCmsPage(StorefrontModel.Cms.CmsPageDefinition cmsPage)
        {
            var result = new CmsPage()
            {
                Settings = cmsPage.Settings,
                Blocks = new List<IDictionary<string, object>>()
            };

            foreach(IDictionary<string, object> block in cmsPage.Blocks)
            {
                MetafieldsCollection collection = new MetafieldsCollection(string.Empty, block);
                result.Blocks.Add(collection);
            }

            return result;
        }
    }
}
