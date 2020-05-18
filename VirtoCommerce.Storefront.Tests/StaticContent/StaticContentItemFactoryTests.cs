using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Model.StaticContent;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.StaticContent
{
    public class StaticContentItemFactoryTests
    {
        private IStaticContentItemFactory factory = new StaticContentItemFactory();

        [Fact]
        public void ContentItem_ShouldBe_BlogArticle()
        {
            var item = factory.GetItemFromPath("blogs/news/article-file.md");
            var article = (BlogArticle)item;
            Assert.Equal(typeof(BlogArticle), item.GetType());
            Assert.Equal("news", article.BlogName);
        }

        [Fact]
        public void ContentItem_ShoudBe_StaticPage()
        {
            var item = factory.GetItemFromPath("custom/path");
            Assert.Equal(typeof(ContentPage), item.GetType());
        }
    }
}
