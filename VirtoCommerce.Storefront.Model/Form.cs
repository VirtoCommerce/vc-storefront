using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class Form : IDictionary<string, object>
    {
        private readonly IDictionary<string, object> _dict;

        public Form()
            : this(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null))
        {
        }

        private Form(IDictionary<string, object> dict)
        {
            _dict = dict;
            PostedSuccessfully = true;
            Errors = new List<string>();
        }

        public static Form FromObject(object obj)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);
            var formProps = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var formPropNames = formProps.Select(x => x.Name).ToArray();
            foreach (var property in formProps)
            {
                var propertyValue = property.GetValue(obj);
                if (propertyValue != null)
                {
                    dict[property.Name] = propertyValue;
                }
            }
            return new Form(dict);
        }

        /// <summary>
        /// Returns an array of strings if the form was not submitted successfully.
        /// The strings returned depend on which fields of the form were left empty or contained errors.
        /// </summary>
        public IList<string> Errors { get; set; }

        /// <summary>
        /// Returns true if the form was submitted successfully, or false if the form contained errors.
        /// All forms but the address form set that property.
        /// The address form is always submitted successfully.
        /// </summary>
        public bool? PostedSuccessfully { get; set; }

        public void Add(string key, object value)
        {
            _dict.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _dict.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _dict.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _dict.Add(item);
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _dict.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _dict.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public ICollection<string> Keys => _dict.Keys;

        public ICollection<object> Values => _dict.Values;

        public int Count => _dict.Count();

        public bool IsReadOnly => _dict.IsReadOnly;

        public object this[string key] { get => _dict[key]; set => _dict[key] = value; }
    }
}
