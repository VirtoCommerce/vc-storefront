using System;

namespace VirtoCommerce.Storefront.Domain
{
    public class ContentItemReaderFactory : IContentItemReaderFactory
    {
        public IContentItemReader CreateReader(string blobRelativePath, string content)
        {
            if (blobRelativePath.EndsWith(".page", StringComparison.InvariantCultureIgnoreCase))
            {
                return new JsonPageReader(blobRelativePath, content);
            }
            return new MarkdownReader(blobRelativePath, content);
        }
    }
}
