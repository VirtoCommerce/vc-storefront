using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.CustomerReviews;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontRoute]
    public class ApiCutomerReviewController : Controller
    {
        private readonly ICustomerReviewService _customerReviewService;

        public ApiCutomerReviewController(ICustomerReviewService customerReviewService)
        {
            _customerReviewService = customerReviewService;
        }

        [HttpPost("customer/review/{reviewId}/upvote")]
        public async Task<ActionResult> Upvote(string reviewId)
        {
            await _customerReviewService.UpvoteAsync(reviewId);
            return Ok();
        }

        [HttpPost("customer/review/{reviewId}/downvote")]
        public async Task<ActionResult> Downvote(string reviewId)
        {
            await _customerReviewService.DownvoteAsync(reviewId);
            return Ok();
        }
    }
}
