using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Model.Common.SearchResults
{
    public class UserSearchResult
    {
        public UserSearchResult()
        {
            Results = new List<User>();
        }

        public int TotalCount { get; set; }
        public ICollection<User> Results { get; set; }
    }
}
