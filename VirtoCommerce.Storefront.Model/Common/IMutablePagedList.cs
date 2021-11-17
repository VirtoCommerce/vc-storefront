using System.Collections.Generic;
using System.Collections.Specialized;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Common
{
    /// <summary>
    /// PagedList which page number and page size can be changed  (on render view time for example)
    /// </summary>
    public interface IMutablePagedList : IPagedList
    {
        /// <summary>
        /// Contains information for sorting order
        /// </summary>
        IEnumerable<SortInfo> SortInfos { get; }
        /// <summary>
        /// Slice  the current set of data by new page sizes
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        void Slice(int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos, NameValueCollection @params = null);
    }

    public interface IMutablePagedList<T> : IMutablePagedList, IPagedList<T>
    {
    }
}
