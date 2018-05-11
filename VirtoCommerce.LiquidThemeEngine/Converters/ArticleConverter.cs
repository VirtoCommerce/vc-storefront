using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using StorefrontModel = VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class ArticleConverter
    {
        public static Article ToShopifyModel(this StorefrontModel.BlogArticle article)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidArticle(article);
        }
    }

    public partial class ShopifyModelConverter
    {
       public virtual Article ToLiquidArticle(StorefrontModel.BlogArticle article)
        {
            var retVal = new Article();

            retVal.Author = article.Author;
            retVal.Category = article.Category;
            retVal.Content = article.Content;
            retVal.Description = article.Description;
            retVal.Excerpt = article.Excerpt;
            retVal.ImageUrl = article.ImageUrl;
            retVal.IsSticked = article.IsSticked;
            retVal.IsTrending = article.IsTrending;
            retVal.Priority = article.Priority;
            retVal.Title = article.Title;
            retVal.Type = article.Type;
            retVal.Url = article.Url;
            retVal.Handle = article.Url;
            retVal.CreatedAt = article.CreatedDate;
            retVal.PublishedAt = article.PublishedDate ?? article.CreatedDate;
            retVal.Tags = article.Tags != null ? article.Tags.OrderBy(t => t).Select(t => t.Handelize()).ToArray() : null;
            retVal.Comments = new MutablePagedList<Comment>(new List<Comment>());
            if (article.Category != null)
            {
                retVal.Category = article.Category.Handelize();
            }
            return retVal;
        }
    }
}