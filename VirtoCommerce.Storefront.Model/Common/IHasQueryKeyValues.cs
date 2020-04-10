using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Common
{
    public interface IHasQueryKeyValues
    {
        IEnumerable<KeyValuePair<string, string>> GetQueryKeyValues();
    }
}
