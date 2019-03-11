using VirtoCommerce.Storefront.Model.CustomerReviews;
using reviewDto = VirtoCommerce.Storefront.AutoRestClients.CustomerReviews.WebModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain.CustomerReview
{
    public static partial class CustomerReviewConverter
    {
        public static Model.CustomerReviews.CustomerReview ToCustomerReview(this reviewDto.CustomerReview itemDto)
        {
            var result = new Model.CustomerReviews.CustomerReview
            {
                Id = itemDto.Id,
                AuthorNickname = itemDto.AuthorNickname,
                Content = itemDto.Content,
                CreatedBy = itemDto.CreatedBy,
                CreatedDate = itemDto.CreatedDate,
                IsActive = itemDto.IsActive ?? false,
                IsRecommended = itemDto.IsRecommended ?? false,
                ModifiedBy = itemDto.ModifiedBy,
                ModifiedDate = itemDto.ModifiedDate,
                ProductId = itemDto.ProductId,
                Rate = itemDto.Rate ?? 0,
                VoteCount = itemDto.VoteCount ?? 0
            };

            return result;
        }


        public static reviewDto.CustomerReviewSearchCriteria ToSearchCriteriaDto(
            this CustomerReviewSearchCriteria criteria)
        {
            var result = new reviewDto.CustomerReviewSearchCriteria
            {
                IsActive = criteria.IsActive,
                ProductIds = criteria.ProductIds,

                Skip = criteria.Start,
                Take = criteria.PageSize,
                Sort = criteria.Sort
            };

            return result;
        }
    }
}
