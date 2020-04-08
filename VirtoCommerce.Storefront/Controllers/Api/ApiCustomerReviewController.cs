using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.CustomerReviews;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("reviews")]
    [ResponseCache(CacheProfileName = "None")]
    public class CustomerReviewController : StorefrontControllerBase
    {
        private readonly ICustomerReviewService _customerReviewService;

        public CustomerReviewController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ICustomerReviewService customerReviewService)
            : base(workContextAccessor, urlBuilder)
        {
            _customerReviewService = customerReviewService;
        }

        [HttpPost("publish")]
        public async Task<ActionResult> CreateCustomerReview([FromBody] CreateCustomerReviewRequest request)
        {
            await _customerReviewService.CreateReviewAsync(request, WorkContext.CurrentProductResponseGroup);
            return Ok();
        }
    }
}
