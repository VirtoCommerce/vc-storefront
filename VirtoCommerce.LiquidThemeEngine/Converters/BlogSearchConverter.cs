using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class BlogSearchConverter
    {
        public static BlogSearch ToShopifyModel(this StorefrontModel.BlogSearchCriteria blogSearchCriteria)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidBlogSearch(blogSearchCriteria);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual BlogSearch ToLiquidBlogSearch(StorefrontModel.BlogSearchCriteria blogSearchCriteria)
        {
            var retVal = new BlogSearch();
            retVal.Author = blogSearchCriteria.Author;
            retVal.Category = blogSearchCriteria.Category;
            retVal.Tag = blogSearchCriteria.Tag;

            return retVal;
        }
    }
}
