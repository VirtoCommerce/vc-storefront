using System.IO;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class StaticContentItemBuilder : IStaticContentItemBuilder
    {
        private readonly IStaticContentItemFactory _factory;
        readonly IContentItemReaderFactory _readerFactory;
        readonly IContentRestorerFactory _restorerFactory;

        public StaticContentItemBuilder(IStaticContentItemFactory factory,
            IContentItemReaderFactory readerFactory, IContentRestorerFactory restorerFactory)
        {
            _factory = factory;
            _readerFactory = readerFactory;
            _restorerFactory = restorerFactory;
        }

        public ContentItem BuildFrom(string baseStoreContentPath, string blobRelativePath, string content)
        {
            var contentItem = _factory.GetItemFromPath(blobRelativePath);
            if (contentItem != null)
            {
                contentItem.StoragePath = "/" +
                        (string.IsNullOrEmpty(baseStoreContentPath) ?
                            blobRelativePath :
                            blobRelativePath.Replace(baseStoreContentPath + "/", string.Empty)
                        ).TrimStart('/');
                contentItem.FileName = Path.GetFileName(blobRelativePath);
                var reader = _readerFactory.CreateReader(blobRelativePath, content);
                var restorer = _restorerFactory.CreateRestorer(blobRelativePath, contentItem);
                restorer.FulfillContent(reader, contentItem);
            }


            return contentItem;
        }
    }
}
