using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Interaction;
using VirtoCommerce.Storefront.Model.Recommendations;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("useractions")]
    public class ApiUserActionsController : StorefrontControllerBase
    {
        private readonly IRecommendationsProvider _productRecommendationsService;

        public ApiUserActionsController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder,
            IRecommendationsProvider productRecommendationsApi) : base(workContextAccessor, urlBuilder)
        {
            _productRecommendationsService = productRecommendationsApi;
        }

        /// <summary>
        /// POST /storefrontapi/useractions/eventinfo
        /// Record user actions events in VC platform
        /// </summary>
        /// <param name="userSession"></param>
        /// <returns></returns>
        [HttpPost("eventinfo")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveEventInfo([FromBody] UserSession userSession)
        {
            //TODO: need to replace to other special detected VC API for usage
            if (userSession.Interactions != null)
            {
                IList<UsageEvent> usageEvents = userSession.Interactions.Select(i => new UsageEvent
                {
                    EventType = i.Type,
                    ItemId = i.Content,
                    CreatedDate = i.CreatedAt,
                    CustomerId = WorkContext.CurrentUser.Id,
                    StoreId = WorkContext.CurrentStore.Id
                }).ToList();

                await _productRecommendationsService.AddEventAsync(usageEvents);
            }

            return Ok();
        }
    }
}
