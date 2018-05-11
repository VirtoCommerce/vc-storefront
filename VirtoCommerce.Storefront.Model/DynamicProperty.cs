using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class DynamicProperty : Entity
    {
        public string Name { get; set; }

        public LocalizedString DisplayName { get; set; }
        public IList<LocalizedString> DisplayNames { get; set; } = new List<LocalizedString>();
        /// <summary>
        /// Defines whether a property supports multiple values.
        /// </summary>
        public bool IsArray { get; set; }
        /// <summary>
        /// Dictionary has a predefined set of values. User can select one or more of them and cannot enter arbitrary values.
        /// </summary>
        public bool IsDictionary { get; set; }

        public bool IsRequired { get; set; }

        public string ValueType { get; set; }

        //Selected scalar values
        public IList<LocalizedString> Values { get; set; } = new List<LocalizedString>();
        //Selected dictionary values
        public IList<DynamicPropertyDictionaryItem> DictionaryValues { get; set; } = new List<DynamicPropertyDictionaryItem>();
        //All possible dictionary values
        public IList<DynamicPropertyDictionaryItem> DictionaryItems { get; set; } = new List<DynamicPropertyDictionaryItem>();
    }

  
}