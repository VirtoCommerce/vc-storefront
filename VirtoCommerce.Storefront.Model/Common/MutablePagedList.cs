using PagedList.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace VirtoCommerce.Storefront.Model.Common
{
    public sealed class MutablePagedList<T> : PagedListMetaData, IMutablePagedList<T>, IDictionary
    {

        private readonly Func<int, int, IEnumerable<SortInfo>, NameValueCollection, IPagedList<T>> _getter;
        private IPagedList<T> _pagedList;
        private readonly object _lockObject = new object();

        public MutablePagedList(IEnumerable<T> superSet)
            : this((newPageNumber, newPageSize, sortInfos) => new PagedList<T>(superSet.AsQueryable(), newPageNumber, newPageSize), 1, Math.Max(superSet.Count(), 1))
        {
            TotalItemCount = superSet.Count();
            PageCount = 1;
        }

        public MutablePagedList(Func<int, int, IEnumerable<SortInfo>, IPagedList<T>> getter, int pageNumber, int pageSize)
            : this((pn, ps, sortInfos, parameters) => getter(pageNumber, pageSize, sortInfos), pageNumber, pageSize)
        {
        }

        public MutablePagedList(Func<int, int, IEnumerable<SortInfo>, NameValueCollection, IPagedList<T>> getter, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            _getter = getter;
        }


        #region IMutablePagedList Members

        public IEnumerable<SortInfo> SortInfos { get; private set; }
        public NameValueCollection Params { get; private set; }
        /// <summary>
        /// Resize current paged data list by new PageNumber and PageSize values (it may cause reloading data from source)
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void Slice(int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos, NameValueCollection @params = null)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "PageNumber cannot be below 1.");
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "PageSize cannot be less than 1.");

            if (pageNumber != PageNumber)
            {
                PageNumber = pageNumber;
                _pagedList = null;
            }
            if (pageSize != PageSize)
            {
                PageSize = pageSize;
                _pagedList = null;
            }

            if (SortInfos != sortInfos)
            {
                SortInfos = sortInfos;
                _pagedList = null;
            }

            if (Params != @params)
            {
                Params = @params;
                _pagedList = null;
            }

            TryReloadPagedData();
        }

        #endregion

        #region IPagedList<T> Members

        /// <summary>
        /// 	Returns an enumerator that iterates through the BasePagedList&lt;T&gt;.
        /// </summary>
        /// <returns>A BasePagedList&lt;T&gt;.Enumerator for the BasePagedList&lt;T&gt;.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            TryReloadPagedData();
            return _pagedList.GetEnumerator();
        }

        /// <summary>
        /// 	Returns an enumerator that iterates through the BasePagedList&lt;T&gt;.
        /// </summary>
        /// <returns>A BasePagedList&lt;T&gt;.Enumerator for the BasePagedList&lt;T&gt;.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ///<summary>
        ///	Gets the element at the specified index.
        ///</summary>
        ///<param name = "index">The zero-based index of the element to get.</param>
        public T this[int index]
        {
            get
            {
                TryReloadPagedData();
                return _pagedList[index];
            }
        }

        /// <summary>
        /// 	Gets the number of elements contained on this page.
        /// </summary>
        public int Count
        {
            get
            {
                TryReloadPagedData();
                return _pagedList.Count;
            }
        }

        ///<summary>
        /// Gets a non-enumerable copy of this paged list.
        ///</summary>
        ///<returns>A non-enumerable copy of this paged list.</returns>
        public IPagedList GetMetaData()
        {
            TryReloadPagedData();
            return new PagedListMetaData(this);
        }

        #endregion

        #region IDictionary members

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_pagedList).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_pagedList).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_pagedList).SyncRoot; }
        }


        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        ICollection IDictionary.Keys
        {
            get
            {
                TryReloadPagedData();
                return _pagedList.OfType<IDictionaryKey>().Select(x => x.Key).ToList();
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                TryReloadPagedData();
                return _pagedList.OfType<IDictionaryKey>().Where(x => !string.IsNullOrEmpty(x.Key)).ToList();
            }
        }


        public object this[object key]
        {
            get
            {
                TryReloadPagedData();
                object result = null;
                if (key is T other)
                {
                    result = _pagedList.FirstOrDefault(x => x.Equals(other));
                }
                if (key is string stringKey)
                {
                    if (stringKey == "size")
                    {
                        result = Count;
                    }
                    else
                    {
                        result = _pagedList.OfType<IDictionaryKey>().Where(x => !string.IsNullOrEmpty(x.Key)).FirstOrDefault(x => x.Key.EqualsInvariant(stringKey));
                    }
                }
                return result;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool IDictionary.Contains(object value)
        {
            TryReloadPagedData();
            var result = false;
            if (value is T other)
            {
                result = _pagedList.Any(x => x.Equals(other));
            }
            if (typeof(IDictionaryKey).IsAssignableFrom(typeof(T)) && value is string key)
            {
                result = _pagedList.OfType<IDictionaryKey>().Where(x => !string.IsNullOrEmpty(x.Key)).Any(x => x.Key.EqualsInvariant(key));
            }
            if (!result && value is string strValue)
            {
                result = strValue == "size";
            }
            return result;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            TryReloadPagedData();
            var dict = _pagedList.OfType<IDictionaryKey>().Where(x => !string.IsNullOrEmpty(x.Key)).ToDictionary(x => x.Key, x => (object)x);
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

        private void TryReloadPagedData()
        {
            if (_pagedList == null)
            {
                lock (_lockObject)
                {
                    if (_pagedList == null)
                    {
                        _pagedList = _getter(PageNumber, PageSize, SortInfos, Params);
                    }
                }
                // set source to blank list if superset is null to prevent exceptions
                TotalItemCount = _pagedList.TotalItemCount;
                PageCount = TotalItemCount > 0
                                ? (int)Math.Ceiling(TotalItemCount / (double)PageSize)
                                : 0;
                HasPreviousPage = PageNumber > 1;
                HasNextPage = PageNumber < PageCount;
                IsFirstPage = PageNumber == 1;
                IsLastPage = PageNumber >= PageCount;
                FirstItemOnPage = (PageNumber - 1) * PageSize + 1;
                var numberOfLastItemOnPage = FirstItemOnPage + PageSize - 1;
                LastItemOnPage = numberOfLastItemOnPage > TotalItemCount
                                     ? TotalItemCount
                                     : numberOfLastItemOnPage;
            }
        }
    }
}
