using DotLiquid;
using PagedList.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public abstract class ItemCollection<T> : Drop, ICollection, ILiquidContains, IEnumerable<T>, IMutablePagedList
        where T : class
    {
        private readonly IMutablePagedList<T> _mutablePagedList;
        public ItemCollection(IEnumerable<T> superset)
        {
            if (superset == null)
            {
                superset = Enumerable.Empty<T>();
            }
            _mutablePagedList = superset as IMutablePagedList<T>;
            if (_mutablePagedList == null)
            {
                _mutablePagedList = new MutablePagedList<T>(superset);
            }
        }
        public long Size
        {
            get
            {
                return _mutablePagedList.Count;
            }
        }
        #region ICollection members
        public object SyncRoot
        {
            get
            {
                return _mutablePagedList;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        public int Count
        {
            get
            {
                return _mutablePagedList.Count;
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IEnumerable<T> Members
        public IEnumerator<T> GetEnumerator()
        {
            return _mutablePagedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _mutablePagedList.GetEnumerator();
        }
        #endregion

        #region ILiquidContains Members
        public virtual bool Contains(object value)
        {
            var other = value as T;
            if (other != null)
            {
                return _mutablePagedList.Any(x => x.Equals(other));
            }
            return false;
        }
        #endregion

        #region IMutablePagedList Members
        public IEnumerable<SortInfo> SortInfos
        {
            get
            {
                return _mutablePagedList.SortInfos;
            }
        }

        public int PageCount
        {
            get
            {
                return _mutablePagedList.PageCount;
            }
        }

        public int TotalItemCount
        {
            get
            {
                return _mutablePagedList.TotalItemCount;
            }
        }

        public int PageNumber
        {
            get
            {
                return _mutablePagedList.PageNumber;
            }
        }

        public int PageSize
        {
            get
            {
                return _mutablePagedList.PageSize;
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return _mutablePagedList.HasPreviousPage;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return _mutablePagedList.HasNextPage;
            }
        }

        public bool IsFirstPage
        {
            get
            {
                return _mutablePagedList.IsFirstPage;
            }
        }

        public bool IsLastPage
        {
            get
            {
                return _mutablePagedList.IsLastPage;
            }
        }

        public int FirstItemOnPage
        {
            get
            {
                return _mutablePagedList.FirstItemOnPage;
            }
        }

        public int LastItemOnPage
        {
            get
            {
                return _mutablePagedList.LastItemOnPage;
            }
        }

        public void Slice(int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos)
        {
            _mutablePagedList.Slice(pageNumber, pageSize, sortInfos);
        }

        public IPagedList GetMetaData()
        {
            return _mutablePagedList.GetMetaData();
        }
        #endregion
    }
}
