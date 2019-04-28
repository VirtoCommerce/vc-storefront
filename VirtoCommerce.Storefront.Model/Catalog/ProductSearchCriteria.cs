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

        private void Parse(NameValueCollection queryString)
        {        
            IsFuzzySearch = queryString.Get("fuzzy").EqualsInvariant(bool.TrueString);
            Keyword = queryString.Get("q") ?? queryString.Get("keyword");
            SortBy = queryString.Get("sort_by");
            ResponseGroup = EnumUtility.SafeParse(queryString.Get("resp_group"), ItemResponseGroup.ItemSmall | ItemResponseGroup.ItemWithPrices | ItemResponseGroup.Inventory | ItemResponseGroup.ItemWithDiscounts | ItemResponseGroup.ItemWithVendor | ItemResponseGroup.ItemProperties);
            // terms=name1:value1,value2,value3;name2:value1,value2,value3
            Terms = (queryString.GetValues("terms") ?? new string[0])
                .SelectMany(s => s.Split(';'))
                .Select(s => s.Split(':'))
                .Where(a => a.Length == 2)
                .SelectMany(a => a[1].Split(',').Select(v => new Term { Name = a[0], Value = v }))
                .ToArray();
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
