using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class StaticContentItemBuilder
    {

        private class PageContentParser : IContentItemParser
        {
            public bool Suit(ContentItem item)
            {
                return item.FileName.EndsWith(".page");
            }

            public void Parse(string path, string content, ContentItem item)
            {
                item.Content = content;
            }
        }
    }
}
