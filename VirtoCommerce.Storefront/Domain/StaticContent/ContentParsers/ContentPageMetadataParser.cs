using System.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class StaticContentItemBuilder
    {

        private class ContentPageMetadataParser : IContentItemParser
        {
            public bool Suit(ContentItem item)
            {
                return item is ContentPage;
            }

            public void Parse(string path, string content, ContentItem item)
            {
                var page = (ContentPage)item;
                page.Template = "page";

                if (page.MetaInfo.ContainsKey("template"))
                {
                    page.Template = page.MetaInfo["template"].FirstOrDefault();
                }

            }
        }
    }
}
