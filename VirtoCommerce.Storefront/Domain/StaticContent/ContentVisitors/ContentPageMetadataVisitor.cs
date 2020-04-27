using System.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal class ContentPageMetadataVisitor : IContentItemVisitor
    {
        public ContentProcessStage Stage => ContentProcessStage.Metadata;
        public bool Suit(ContentItem item)
        {
            return item is ContentPage;
        }

        public string ReadContent(string path, string content, ContentItem item)
        {
            var page = (ContentPage)item;
            page.Template = "page";

            if (page.MetaInfo.ContainsKey("template"))
            {
                page.Template = page.MetaInfo["template"].FirstOrDefault();
            }
            return content;
        }
    }
}
