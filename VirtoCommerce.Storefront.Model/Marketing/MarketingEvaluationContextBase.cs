using System;
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
            //Remove user for equality because marketing promotions very rarely depend on concrete customer and exclude  user from  cache key can have significant affect to performance
            //yield return User;
            yield return string.Join('&', User?.Contact?.UserGroups ?? Array.Empty<string>());
            yield return string.Join('&', User?.Contact?.OrganizationsIds ?? Array.Empty<string>());
        }

    }
}
