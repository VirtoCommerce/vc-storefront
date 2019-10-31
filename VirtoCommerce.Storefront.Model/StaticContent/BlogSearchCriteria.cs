using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public partial class BlogSearchCriteria : PagedSearchCriteria
    {
        private static int _defaultPageSize = 20;

        public static int DefaultPageSize
        {
            get { return _defaultPageSize; }
            set { _defaultPageSize = value; }
        }

        public BlogSearchCriteria()
            : this(new NameValueCollection())
        {
        }

        public BlogSearchCriteria(NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
        }

        public string Category { get; set; }
        public string Tag { get; set; }
        public string Author { get; set; }
        public string[] ExcludedArticleHandles { get; set; }

        protected virtual void Parse(NameValueCollection queryString)
        {
            Category = queryString.Get("category");
            Tag = queryString.Get("tag");
            Author = queryString.Get("author");
        }
    }
}
