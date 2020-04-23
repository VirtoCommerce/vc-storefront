using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class ProductSearchCriteria : PagedSearchCriteria, IHasQueryKeyValues
    {
        public static int DefaultPageSize { get; set; } = 20;

        //For JSON deserialization
        public ProductSearchCriteria()
            : this(null, null)
        {
        }

        public ProductSearchCriteria(Language language, Currency currency)
            : this(language, currency, new NameValueCollection())
        {
        }

        public ProductSearchCriteria(Language language, Currency currency, NameValueCollection queryString)
            : base(queryString, DefaultPageSize)
        {
            Language = language;
            Currency = currency;

            Parse(queryString);
        }

        public ItemResponseGroup ResponseGroup { get; set; }

        public string Outline { get; set; }

        public Currency Currency { get; set; }

        public NumericRange PriceRange { get; set; }

        public Language Language { get; set; }

        public string Keyword { get; set; }

        public IList<Term> Terms { get; set; } = new List<Term>();

        public IList<string> UserGroups { get; set; } = new List<string>();

        public string SortBy { get; set; }

        public string VendorId { get; set; }

        public bool IsFuzzySearch { get; set; }

        public override object Clone()
        {
            var result = base.Clone() as ProductSearchCriteria;
            if (Terms != null)
            {
                result.Terms = Terms.Select(x => new Term { Name = x.Name, Value = x.Value }).ToList();
            }

            return result;
        }
        public override IEnumerable<KeyValuePair<string, string>> GetQueryKeyValues()
        {
            foreach (var basePair in base.GetQueryKeyValues())
            {
                yield return basePair;
            }
            //Need to return all keys even with null values in order to get known all keys and be able to remove them from the query string
            yield return new KeyValuePair<string, string>("keyword", Keyword);
            yield return new KeyValuePair<string, string>("sort_by", SortBy);
            yield return new KeyValuePair<string, string>("terms", !Terms.IsNullOrEmpty() ? string.Join(";", Terms.ToStrings()) : null);
        }

        private void Parse(NameValueCollection queryString)
        {        
            IsFuzzySearch = queryString.Get("fuzzy").EqualsInvariant(bool.TrueString);
            Keyword = queryString.Get("q") ?? queryString.Get("keyword");
            SortBy = queryString.Get("sort_by");

            ResponseGroup = EnumUtility.SafeParse(queryString.Get("resp_group"), ItemResponseGroup.Default);
            // terms=name1:value1,value2,value3;name2:value1,value2,value3
            Terms = (queryString.GetValues("terms") ?? Array.Empty<string>()).SelectMany(x => x.ToTerms()).ToList();
        }

        public override string ToString()
        {
            var result = new List<string>
            {
                string.Format(CultureInfo.InvariantCulture, "page={0}", PageNumber),
                string.Format(CultureInfo.InvariantCulture, "page_size={0}", PageSize)
            };

            if (Keyword != null)
            {
                result.Add(string.Format(CultureInfo.InvariantCulture, "q={0}", Keyword));
            }

            return string.Join("&", result);
        }
    }
}
