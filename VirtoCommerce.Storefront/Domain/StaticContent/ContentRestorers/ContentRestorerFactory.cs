using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public class ContentRestorerFactory : IContentRestorerFactory
    {
        public IContentItemRestorer CreateRestorer(string blobRelativePath, ContentItem item)
        {
            if (item.GetType() == typeof(BlogArticle))
            {
                return new BlogArticleRestorer();
            }
            return new StaticPageRestorer();
        }
    }
}
