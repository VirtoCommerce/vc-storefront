using System;
using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Common
{
    public class PagedSearchCriteria : ValueObject
    {
        public PagedSearchCriteria(IDictionary<string, string> queryString, int defaultPageSize)
        {
            PageNumber = Convert.ToInt32(queryString["page"] ?? 1.ToString());
            PageSize = Convert.ToInt32(queryString["count"] ?? queryString["page_size"] ?? defaultPageSize.ToString());
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

        public virtual IEnumerable<KeyValuePair<string, string>> GetQueryKeyValues()
        {
            yield return new KeyValuePair<string, string>("page", PageNumber.ToString());
            yield return new KeyValuePair<string, string>("page_size", PageSize.ToString());
        }

    }
}
