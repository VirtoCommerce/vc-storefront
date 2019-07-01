using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Marketing
{
    /// <summary>
    /// Represents context object for dynamic content evaluation
    /// </summary>
    public partial class DynamicContentEvaluationContext : MarketingEvaluationContextBase
    {
        public DynamicContentEvaluationContext(Language language, Currency currency)
            : base(language, currency)
        {
        }
        public string PlaceName { get; set; }
        public string[] Tags { get; set; }

        public DateTime? ToDate { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var baseItem in base.GetEqualityComponents())
            {
                yield return baseItem;
            }
            yield return PlaceName;
            yield return ToDate;
            yield return string.Join('&', Tags ?? Array.Empty<string>());
        }
    }
}
