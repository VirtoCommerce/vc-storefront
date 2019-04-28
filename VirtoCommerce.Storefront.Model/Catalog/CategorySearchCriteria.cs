using System.Collections.Generic;
using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class CategorySearchCriteria : PagedSearchCriteria
    {

        public static int DefaultPageSize { get; set; } = 20;

        //For JSON deserialization 
        public CategorySearchCriteria()
            : this(null)
        {
        }

        public CategorySearchCriteria(Language language)
            : this(language, new NameValueCollection())
        {
        }

        public CategorySearchCriteria(Language language, NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
            Language = language;
            Parse(queryString);
        }

        public CategoryResponseGroup ResponseGroup { get; set; }

        public string Outline { get; set; }

        public Language Language { get; set; }

        public string Keyword { get; set; }

        public string SortBy { get; set; }

        public bool IsFuzzySearch { get; set; }


        private void Parse(NameValueCollection queryString)
        {
            IsFuzzySearch = queryString.Get("fuzzy").EqualsInvariant(bool.FalseString);
            Keyword = queryString.Get("q");
            SortBy = queryString.Get("sort_by");
            ResponseGroup = EnumUtility.SafeParse<CategoryResponseGroup>(queryString.Get("resp_group"), CategoryResponseGroup.Small);
        }

        public override string ToString()
        {
            var retVal = new List<string>
            {
                string.Format("page={0}", PageNumber),
                string.Format("page_size={0}", PageSize)
            };
            if (Keyword != null)
            {
                retVal.Add(string.Format("q={0}", Keyword));
            }
            return string.Join("&", retVal);
        }
    }
}
