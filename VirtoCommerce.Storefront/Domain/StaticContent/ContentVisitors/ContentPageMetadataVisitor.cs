using System.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal class ContentPageMetadataVisitor : IContentItemVisitor
    {
        public bool Suit(ContentItem item)
        {
            return item is ContentPage;
        }

        public ContentItem Parse(string path, string content, ContentItem item)
        {
            var page = (ContentPage)item;
            page.Template = "page";

            if (page.MetaInfo.ContainsKey("template"))
            {
                page.Template = page.MetaInfo["template"].FirstOrDefault();
            }
            return item;
        }
    }
}
