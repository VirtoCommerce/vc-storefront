using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class ProductSearchCriteria : PagedSearchCriteria
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
                result.Terms = Terms.Select(x => new Term { Name = x.Name, Value = x.Value }).ToArray();
            }

            return result;
        }
        public override IEnumerable<KeyValuePair<string, string>> GetKeyValues()
        {
			 foreach (var basePair in base.GetKeyValues())
            {
                yield return basePair;
            }
            if (!string.IsNullOrEmpty(Keyword))
            {
                yield return new KeyValuePair<string, string>("keyword", Keyword);
            }
            if (!string.IsNullOrEmpty(SortBy))
            {
                yield return new KeyValuePair<string, string>("sort_by", SortBy);
            }
            if (!Terms.IsNullOrEmpty())
            {
                var termsString = string.Join(";", Terms.ToStrings());
                yield return new KeyValuePair<string, string>("terms", termsString);
            }

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
