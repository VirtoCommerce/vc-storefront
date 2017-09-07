using System;
using System.Collections.Specialized;

namespace VirtoCommerce.Storefront.Model.Common
{
    public class PagedSearchCriteria
    {
        public PagedSearchCriteria(NameValueCollection queryString, int defaultPageSize)
        {
            PageNumber = Convert.ToInt32(queryString.Get("page") ?? 1.ToString());
            PageSize = Convert.ToInt32(queryString.Get("count") ?? queryString.Get("page_size") ?? defaultPageSize.ToString());
        }

        public int Start
        {
            get
            {
                return (PageNumber - 1) * PageSize;
            }
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;

                hash = hash * 59 + PageNumber.GetHashCode();
                hash = hash * 59 + PageSize.GetHashCode();

                return hash;
            }
        }
    }
}
