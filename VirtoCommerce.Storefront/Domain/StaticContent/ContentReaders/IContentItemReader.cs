using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Domain
{
    public interface IContentItemReader
    {
        Dictionary<string, object> ReadMetadata();
        string ReadContent();
    }
}
