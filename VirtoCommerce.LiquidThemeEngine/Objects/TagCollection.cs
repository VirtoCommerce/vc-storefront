using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class TagCollection : IScriptObject, IList, IDictionary
    {
        private readonly IMutablePagedList<Tag> _tags;
        public TagCollection(IMutablePagedList<Tag> tags)
        {
            _tags = tags;
        }

        public IEnumerable<string> Groups
        {
            get
            {
                var retVal = _tags.GroupBy(t => t.GroupLabel).Select(g => g.Key);
                return retVal;
            }
        }

        public int Count => _tags.Count();

        public bool IsReadOnly { get => true; set => throw new System.NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsSynchronized => false;

        public object SyncRoot => _tags;

        public ICollection Keys => throw new NotImplementedException();

        public ICollection Values => throw new NotImplementedException();

        public object this[object key]
        {
            get
            {
                return _tags.FirstOrDefault(x => x.Equals(key));
            }
            set
            {

                throw new NotImplementedException();
            }
        }

        public object this[int index]
        {
            get
            {
                return _tags[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<string> GetMembers()
        {
            return _tags.Select(x => x.Value);
        }

        public bool Contains(string member)
        {
            var result = _tags.Any(x => x.Value.EqualsInvariant(member));
            if (!result)
            {
                result = member.EqualsInvariant("groups") || member.EqualsInvariant("size");
            }
            return result;
        }

        public bool TryGetValue(TemplateContext context, SourceSpan span, string member, out object value)
        {
            value = null;
            value = _tags.FirstOrDefault(x => x.Value.EqualsInvariant(member));
            if (value == null && member.EqualsInvariant("groups"))
            {
                value = Groups;
            }
            if (value == null && member.EqualsInvariant("size"))
            {
                value = _tags.Count;
            }
            return value != null;
        }

        public bool CanWrite(string member)
        {
            return false;
        }

        public void SetValue(TemplateContext context, SourceSpan span, string member, object value, bool readOnly)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(string member)
        {
            throw new System.NotImplementedException();
        }

        public void SetReadOnly(string member, bool readOnly)
        {
            throw new System.NotImplementedException();
        }

        public IScriptObject Clone(bool deep)
        {
            return this;
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return _tags.Contains(value as Tag);
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
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

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return _tags.GetEnumerator();
        }

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
