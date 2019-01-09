using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class Currency : ValueObject
    {
        public string CurrencyCode { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string EnglishName { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }
    }
}
