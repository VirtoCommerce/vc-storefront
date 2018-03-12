using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Text;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class ShoppingCartSearchCriteria : PagedSearchCriteria
    {
        private static int _defaultPageSize = 20;

        public static int DefaultPageSize
        {
            get { return _defaultPageSize; }
            set { _defaultPageSize = value; }
        }

        public ShoppingCartSearchCriteria()
            : base(new NameValueCollection(), _defaultPageSize)
        {
        }

        public ShoppingCartSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }

        public string Sort { get; set; }

        public string StoreId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        [IgnoreDataMember]
        public User Customer { get; set; }

        [IgnoreDataMember]
        public Currency Currency { get; set; }

        [IgnoreDataMember]
        public Language Language { get; set; }
    }
}
