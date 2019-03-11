using System.Threading.Tasks;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.CustomerReviews
{
    public interface ICustomerReviewService
    {
        IPagedList<CustomerReview> SearchReviews(CustomerReviewSearchCriteria criteria);
        Task<IPagedList<CustomerReview>> SearchReviewsAsync(CustomerReviewSearchCriteria criteria);

        void Upvote(string reviewId);
        Task UpvoteAsync(string reviewId);

        void Downvote(string reviewId);
        Task DownvoteAsync(string reviewId);
    }
}
