using System.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal class BlogMetadataVisitor : IContentItemVisitor
    {
        public ContentProcessStage Stage => ContentProcessStage.Metadata;
        public bool Suit(ContentItem item)
        {
            return item is BlogArticle;
        }

        public string ReadContent(string path, string content, ContentItem item)
        {
            var post = (BlogArticle)item;

            if (post.MetaInfo.ContainsKey("main-image"))
            {
                post.ImageUrl = post.MetaInfo["main-image"].FirstOrDefault();
            }

            if (post.MetaInfo.ContainsKey("excerpt"))
            {
                post.Excerpt = post.MetaInfo["excerpt"].FirstOrDefault();
            }

            if (post.MetaInfo.ContainsKey("is-sticked"))
            {
                bool.TryParse(post.MetaInfo["is-sticked"].FirstOrDefault(), out var isSticked);
                post.IsSticked = isSticked;
            }

            if (post.MetaInfo.ContainsKey("is-trending"))
            {
                bool.TryParse(post.MetaInfo["is-trending"].FirstOrDefault(), out var isTrending);

                post.IsTrending = isTrending;
            }

            post.Template = "article";
            return content;
        }
    }
}
