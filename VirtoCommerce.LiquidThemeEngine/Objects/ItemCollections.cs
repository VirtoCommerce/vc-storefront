using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public abstract class ItemCollection<T> : IDictionary, ICollection, ILiquidContains, IEnumerable<T>, IMutablePagedList
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

        protected abstract string GetKey(T obj);

        public IPagedList GetMetaData()
        {
            return _mutablePagedList.GetMetaData();
        }

        #region IDictionary members

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        ICollection IDictionary.Keys => _mutablePagedList.Select(x => GetKey(x)).ToList();

        ICollection IDictionary.Values => _mutablePagedList.ToList();


        public object this[object key]
        {
            get
            {
                object result = null;
                if (key is T other)
                {
                    result = _mutablePagedList.FirstOrDefault(x => x.Equals(other));
                }
                if (key is string stringKey)
                {
                    result = _mutablePagedList.FirstOrDefault(x => GetKey(x).EqualsInvariant(stringKey));
                }
                return result;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool Contains(object value)
        {
            var result = false;
            if (value is T other)
            {
                result = _mutablePagedList.Any(x => x.Equals(other));
            }
            if (value is string key)
            {
                result = _mutablePagedList.Any(x => GetKey(x).EqualsInvariant(key));
            }
            return result;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            var dict = _mutablePagedList.ToDictionary(x => GetKey(x), x => (object)x);
            return dict.GetEnumerator();
        }

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
        }
        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
        #endregion


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



        public void Slice(int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos, NameValueCollection @params = null)
        {
            _mutablePagedList.Slice(pageNumber, pageSize, sortInfos, @params);
        }



        #endregion
    }
}
