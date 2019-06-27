using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Scriban
{
    public class DictionarySemanticList : IScriptObject, IList, IDictionary
    {
        private IList _store;
        public DictionarySemanticList(IList store)
        {
            _store = store;
        }

        #region IScriptObject
        public bool Contains(string member)
        {
            return _store.OfType<IAccessibleByIndexKey>().Any(x => x.IndexKey.EqualsInvariant(member));
        }

        public IEnumerable<string> GetMembers()
        {
            return _store.OfType<IAccessibleByIndexKey>().Select(x => x.IndexKey);
        }

        public void SetValue(TemplateContext context, SourceSpan span, string member, object value, bool readOnly)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TemplateContext context, SourceSpan span, string member, out object value)
        {
            value = _store.OfType<IAccessibleByIndexKey>().FirstOrDefault(x => x.IndexKey.EqualsInvariant(member));
            return value != null;
        }

        public bool Remove(string member)
        {
            throw new NotImplementedException();
        }
        public void SetReadOnly(string member, bool readOnly)
        {
            throw new NotImplementedException();
        }
        public IScriptObject Clone(bool deep)
        {
            return this;
        }

        #endregion

        #region IList
        public object this[int index] { get => _store[index]; set => throw new NotImplementedException(); }

        public bool IsFixedSize => _store.IsFixedSize;

        public bool IsReadOnly => _store.IsReadOnly;

        public int Count => _store.Count;

        public bool IsSynchronized => _store.IsSynchronized;

        public object SyncRoot => _store;

        bool IScriptObject.IsReadOnly { get => true; set => throw new NotImplementedException(); }



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
            return _store.Contains(value);
        }


        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return _store.GetEnumerator();
        }


        public int IndexOf(object value)
        {
            return _store.IndexOf(value);
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

        public ICollection Keys => _store.OfType<IAccessibleByIndexKey>().Select(x => x.IndexKey).ToList();
        public ICollection Values => _store.OfType<IAccessibleByIndexKey>().Select(x => x).ToList();

        public object this[object key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                object result = null;
                if (key is string stringKey)
                {
                    if (stringKey == "size")
                    {
                        result = Count;
                    }
                    else
                    {
                        result = _store.OfType<IAccessibleByIndexKey>().Where(x => !string.IsNullOrEmpty(x.IndexKey)).FirstOrDefault(x => x.IndexKey.EqualsInvariant(stringKey));
                    }
                }
                else if (key is IAccessibleByIndexKey accessibleByIndexKey)
                {
                    result = _store.OfType<IAccessibleByIndexKey>().FirstOrDefault(x => x.IndexKey.EqualsInvariant(accessibleByIndexKey.IndexKey));
                }
                else
                {
                    result = _store.OfType<object>().FirstOrDefault(x => x.Equals(key));
                }
                return result;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

    }
}
