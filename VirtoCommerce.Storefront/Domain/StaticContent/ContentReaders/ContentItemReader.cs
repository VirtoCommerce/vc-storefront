using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VirtoCommerce.Storefront.Domain
{
    public abstract class ContentItemReader : IContentItemReader
    {

        public ContentItemReader(string blobRelativePath, string content)
        {
            BlobRelativePath = blobRelativePath;
            Content = content;
        }

        protected string BlobRelativePath { get; }
        protected string Content { get; }

        public abstract string ReadContent();

        public abstract Dictionary<string, object> ReadMetadata();

        protected virtual void AddPropertiesFromFilename(Dictionary<string, object> metadata)
        {
            // each content file  has a name pattern {name}.{language?}.{ext}
            var parts = Path.GetFileName(BlobRelativePath)?.Split('.');

            if (parts?.Length == 3)
            {
                metadata.Add("language", parts[1]);
                metadata.Add("contentItemName", parts.FirstOrDefault());
            }
        }
    }
}
