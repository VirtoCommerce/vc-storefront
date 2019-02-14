using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Model.Marketing
{
    /// <summary>
    /// Represents context object for promotion evaluation
    /// </summary>
    public abstract class MarketingEvaluationContextBase : ValueObject
    {
        protected MarketingEvaluationContextBase(Language language, Currency currency)
        {
            Language = language;
            Currency = currency;
        }

        public string StoreId { get; set; }
        public Language Language { get; set; }
        public Currency Currency { get; set; }
        public User User { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StoreId;
            yield return Language;
            yield return Currency;
            yield return User;
        }

    }
}
