using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Lists.Services;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using VirtoCommerce.Storefront.Model.Tax.Services;

namespace VirtoCommerce.Storefront.Domain.Lists
{
    public class WishlistBuilder : CartBuilder, IWishlistBuilder
    {
        protected readonly IWorkContextAccessor _workContextAccessor;

        public WishlistBuilder(IWorkContextAccessor workContextAccessor, ICartModule cartApi, ICatalogService catalogSearchService, IMemoryCache memoryCache, IPromotionEvaluator promotionEvaluator, ITaxEvaluator taxEvaluator, ISubscriptionService subscriptionService) :
            base(workContextAccessor, cartApi, catalogSearchService, memoryCache, promotionEvaluator, taxEvaluator, subscriptionService)
        {
            _workContextAccessor = workContextAccessor;
        }

        protected override async Task AddLineItemAsync(LineItem lineItem)
        {
            await base.AddLineItemAsync(lineItem);

            WishlistCacheRegion.ExpireSearchResults(_workContextAccessor.WorkContext.CurrentUser.Id);
        }
    }
}
