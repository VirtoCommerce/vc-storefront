using System.Threading.Tasks;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.CustomerReviews
{
    /// <summary>
    /// Service for interaction with <seealso cref="CustomerReview" >CustomerReviews</seealso>
    /// </summary>
    public interface ICustomerReviewService
    {
        /// <summary>
        /// Synchronously search customer reviews by search criteria
        /// </summary>
        /// <param name="criteria">Search criteria</param>
        /// <returns>Pagebale collection of customer reviews</returns>
        IPagedList<CustomerReview> SearchReviews(CustomerReviewSearchCriteria criteria);

        /// <summary>
        /// Asynchronously search customer reviews by search criteria
        /// </summary>
        /// <param name="criteria">Search criteria</param>
        /// <returns>Pagebale collection of customer reviews</returns>
        Task<IPagedList<CustomerReview>> SearchReviewsAsync(CustomerReviewSearchCriteria criteria);

        Task CreateReviewAsync(CreateCustomerReviewRequest request, ItemResponseGroup responseGroup);
    }
}
