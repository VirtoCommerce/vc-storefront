using System.Collections.Generic;
using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public partial class BlogSearchCriteria : PagedSearchCriteria
    {
        public static int DefaultPageSize { get; set; } = 20;

        public BlogSearchCriteria()
            : this(new Dictionary<string, string>())
        {
        }

        public BlogSearchCriteria(IDictionary<string, string> queryString)
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
