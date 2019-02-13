using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class OrganizationUsersSearchResult
    {
        public int? TotalCount { get; set; }

        /// <summary>
        /// </summary>
        public IList<User> Results { get; set; }
    }
}
