using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Domain
{
    public interface IContentItemReader
    {
        Dictionary<string, IEnumerable<string>> ReadMetadata();
        string ReadContent();
    }
}
