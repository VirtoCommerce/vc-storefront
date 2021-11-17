using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Common
{
    public sealed class MutablePagedList<T> : PagedListMetaData, IMutablePagedList<T>, IList, IDictionary
    {
        private static readonly MutablePagedList<T> _empty = new MutablePagedList<T>(Enumerable.Empty<T>());
        private readonly Func<int, int, IEnumerable<SortInfo>, NameValueCollection, IPagedList<T>> _getter;
        private IPagedList<T> _pagedList;
        private readonly object _lockObject = new object();

        public MutablePagedList(IEnumerable<T> superSet, int pageNumber, int pageSize, int totalCount)
          : this((newPageNumber, newPageSize, sortInfos) => new StaticPagedList<T>(superSet.AsQueryable(), newPageNumber, newPageSize, totalCount), pageNumber, pageSize)
        {
            TotalItemCount = totalCount;
            PageCount = 1;
        }

        public MutablePagedList(IEnumerable<T> superSet)
            : this(superSet, 1, 1, superSet.Count())
        {
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

        public static MutablePagedList<T> Empty
        {
            get
            {
                return _empty;
            }
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

            ReloadPagedData();
        }

        #endregion

        #region IPagedList<T> Members

        /// <summary>
        /// 	Returns an enumerator that iterates through the BasePagedList&lt;T&gt;.
        /// </summary>
        /// <returns>A BasePagedList&lt;T&gt;.Enumerator for the BasePagedList&lt;T&gt;.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            ReloadPagedData();
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
                ReloadPagedData();
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
                ReloadPagedData();
                return _pagedList.Count;
            }
        }

        ///<summary>
        /// Gets a non-enumerable copy of this paged list.
        ///</summary>
        ///<returns>A non-enumerable copy of this paged list.</returns>
        public IPagedList GetMetaData()
        {
            ReloadPagedData();
            return new PagedListMetaData(this);
        }

        #endregion

        #region IList
        object IList.this[int index]
        {
            get
            {
                ReloadPagedData();
                return _pagedList.OfType<object>().ToList()[index];

            }
            set => throw new NotImplementedException();
        }

        public bool IsFixedSize => false;

        public bool IsReadOnly => true;


        public bool IsSynchronized => false;

        public object SyncRoot => _pagedList;



        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public bool CanWrite(string member)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }


        public bool Contains(object value)
        {
            return TryGetValue(value, out var dummy);
        }


        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }


        public int IndexOf(object value)
        {
            var result = -1;
            if (TryGetValue(value, out var obj))
            {
                result = _pagedList.OfType<object>().ToList().IndexOf(obj);
            }
            return result;
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
        }


        #endregion
        #region IDictionary

        public ICollection Keys
        {
            get
            {
                ReloadPagedData();
                var dictionary = _pagedList.OfType<IAccessibleByIndexKey>().ToDictionary(x => x.IndexKey, x => x);
                return dictionary.Keys;

            }
        }
        public ICollection Values
        {
            get
            {
                ReloadPagedData();
                var dictionary = _pagedList.OfType<IAccessibleByIndexKey>().ToDictionary(x => x.IndexKey, x => x);
                return dictionary.Values;
            }
        }



        public object this[object key]
        {
            get
            {
                TryGetValue(key, out var result);
                return result;
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            ReloadPagedData();
            var dictionary = _pagedList.OfType<IAccessibleByIndexKey>().ToDictionary(x => x.IndexKey, x => x);
            return dictionary.GetEnumerator();
        }
        #endregion

        private bool TryGetValue(object key, out object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            value = null;
            ReloadPagedData();
            if (key is string stringKey)
            {
                if (stringKey == "size")
                {
                    value = Count;
                }
                else
                {
                    value = _pagedList.OfType<IAccessibleByIndexKey>().Where(x => !string.IsNullOrEmpty(x.IndexKey)).FirstOrDefault(x => x.IndexKey.EqualsInvariant(stringKey));
                }
            }
            else if (key is IAccessibleByIndexKey accessibleByIndexKey)
            {
                value = _pagedList.OfType<IAccessibleByIndexKey>().FirstOrDefault(x => x.IndexKey.EqualsInvariant(accessibleByIndexKey.IndexKey));
            }
            else
            {
                value = _pagedList.FirstOrDefault(x => x.Equals(key));
            }
            return value != null;
        }

        private void ReloadPagedData()
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
