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
            var retVal = new Article
            {
                Author = article.Author,
                Category = article.Category,
                Content = article.Content,
                Description = article.Description,
                Excerpt = article.Excerpt,
                ImageUrl = article.ImageUrl,
                IsSticked = article.IsSticked,
                IsTrending = article.IsTrending,
                Priority = article.Priority,
                Title = article.Title,
                Type = article.Type,
                Url = article.Url,
                Handle = article.Url,
                Id = article.Url,
                CreatedAt = article.CreatedDate,
                PublishedAt = article.PublishedDate ?? article.CreatedDate,
                Tags = article.Tags != null ? article.Tags.OrderBy(t => t).Select(t => t.Handelize()).ToArray() : null,
                Comments = new MutablePagedList<Comment>(new List<Comment>())
            };
            if (article.Category != null)
            {
                retVal.Category = article.Category.Handelize();
            }
            return retVal;
        }
    }
}
