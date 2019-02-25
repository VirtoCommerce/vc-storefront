using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Common
{
    public class GenericSearchResult<T>
    {
        public GenericSearchResult()
        {
            Results = new List<T>();
        }

        public int TotalCount { get; set; }
        public ICollection<T> Results { get; set; }
    }
}
