using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public class BlogArticleRestorer : ContentItemRestorer
    {
        protected override void ApplyMetadata(Dictionary<string, IEnumerable<string>> metadata, ContentItem contentItem)
        {
            base.ApplyMetadata(metadata, contentItem);
            var article = (BlogArticle)contentItem;
            SetIfExists(contentItem, "excerpt", () => article.Excerpt);
            SetIfExists(contentItem, "main-image", () => article.ImageUrl);
            SetIfExists(contentItem, "is-sticked", () => article.IsSticked);
            SetIfExists(contentItem, "is-trending", () => article.IsTrending);
        }
    }
}
