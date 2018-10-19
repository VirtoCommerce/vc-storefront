using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Marketing
{
    public interface IDiscountable
    {
        Currency Currency { get; }

        IList<Discount> Discounts { get; }

        void ApplyRewards(IEnumerable<PromotionReward> rewards);
    }
}
