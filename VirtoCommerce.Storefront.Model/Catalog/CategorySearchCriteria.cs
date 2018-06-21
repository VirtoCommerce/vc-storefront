using System.Collections.Generic;
using System.Collections.Specialized;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class CategorySearchCriteria : PagedSearchCriteria
    {
        private static int _defaultPageSize = 20;

        public static int DefaultPageSize
        {
            get { return _defaultPageSize; }
            set { _defaultPageSize = value; }
        }

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

        public CategorySearchCriteria Clone()
        {
            var retVal = new CategorySearchCriteria(Language)
            {
                Outline = Outline,
                Keyword = Keyword,
                SortBy = SortBy,
                PageNumber = PageNumber,
                PageSize = PageSize,
                ResponseGroup = ResponseGroup,
                IsFuzzySearch = IsFuzzySearch,
            };
            return retVal;
        }
      

        private void Parse(NameValueCollection queryString)
        {
            Keyword = queryString.Get("q");
            SortBy = queryString.Get("sort_by");
            ResponseGroup = EnumUtility.SafeParse<CategoryResponseGroup>(queryString.Get("resp_group"), CategoryResponseGroup.Small);
        }      

        public override string ToString()
        {
            var retVal = new List<string>();
            retVal.Add(string.Format("page={0}", PageNumber));
            retVal.Add(string.Format("page_size={0}", PageSize));
            if (Keyword != null)
            {
                retVal.Add(string.Format("q={0}", Keyword));
            }
            return string.Join("&", retVal);
        }
    }
}
