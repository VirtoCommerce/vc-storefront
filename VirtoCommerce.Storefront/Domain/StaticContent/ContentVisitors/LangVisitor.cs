using System.IO;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    internal class LangVisitor : IContentItemVisitor
    {
        public bool Suit(ContentItem item)
        {
            return true;
        }

        public ContentItem Parse(string path, string content, ContentItem item)
        {
            // each content file  has a name pattern {name}.{language?}.{ext}
            var parts = Path.GetFileName(path)?.Split('.');

            if (parts?.Length == 3)
            {
                try
                {
                    item.Language = new Language(parts[1]);
                }
                catch
                {
                    // ignored
                }
            }
            if (item.Language == null)
            {
                item.Language = Language.InvariantLanguage;
            }
            if (item.Name == null)
            {
                item.Name = parts?.FirstOrDefault();
            }
            return item;
        }
    }
}
