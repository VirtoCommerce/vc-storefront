using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class Form : IDictionary
    {
        private readonly IDictionary<string, object> _dict;

        public Form()
            : this(new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null))
        {
        }

        private Form(IDictionary<string, object> dict)
        {
            _dict = dict;
            Errors = new List<FormError>();
            PostedSuccessfully = true;
        }

        public static Form FromObject(object obj)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);
            var formProps = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in formProps)
            {
                var propertyValue = property.GetValue(obj);
                if (propertyValue != null)
                {
                    dict[property.Name.PascalToKebabCase()] = propertyValue;
                }
            }
            return new Form(dict);
        }

        public void Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object key)
        {
            return TryGetValue(key, out var dummy);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an array of errors if the form was not submitted successfully.
        /// The error returned depend on which fields of the form were left empty or contained errors.
        /// </summary>
        public IList<FormError> Errors { get { return _dict["errors"] as IList<FormError>; } set { _dict["errors"] = value; } }

        /// <summary>
        /// Returns true if the form was submitted successfully, or false if the form contained errors.
        /// All forms but the address form set that property.
        /// The address form is always submitted successfully.
        /// </summary>
        public bool? PostedSuccessfully { get { return _dict["posted_successfully"] as bool?; } set { _dict["posted_successfully"] = value; } }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public ICollection Keys => throw new NotImplementedException();

        public ICollection Values => throw new NotImplementedException();

        public int Count => _dict.Count();

        public bool IsSynchronized => false;

        public object SyncRoot => _dict;

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

        private bool TryGetValue(object key, out object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            value = null;

            if (key is string stringKey)
            {
                if (stringKey == "size")
                {
                    value = Count;
                }
                else
                {
                    value = _dict[stringKey];
                }
            }

            return value != null;
        }

    }
}
