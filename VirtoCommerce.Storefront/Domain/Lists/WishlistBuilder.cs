using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Lists.Services;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using VirtoCommerce.Storefront.Model.Tax.Services;

namespace VirtoCommerce.Storefront.Domain.Lists
{
    public class WishlistBuilder : CartBuilder, IWishlistBuilder
    {
        public WishlistBuilder(IWorkContextAccessor workContextAccessor, ICartModule cartApi, ICatalogService catalogSearchService, IMemoryCache memoryCache, IPromotionEvaluator promotionEvaluator, ITaxEvaluator taxEvaluator, ISubscriptionService subscriptionService) :
            base(workContextAccessor, cartApi, catalogSearchService, memoryCache, promotionEvaluator, taxEvaluator, subscriptionService)
        {
        }
    }
}
