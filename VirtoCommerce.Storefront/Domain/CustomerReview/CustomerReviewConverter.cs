using VirtoCommerce.Storefront.Model.CustomerReviews;
using reviewDto = VirtoCommerce.Storefront.AutoRestClients.CustomerReviewsModuleModuleApi.Models;

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
                IsActive = itemDto.IsActive,
                ModifiedBy = itemDto.ModifiedBy,
                ModifiedDate = itemDto.ModifiedDate,
                ProductId = itemDto.ProductId,
                Raiting = itemDto.Raiting ?? default,
                Likes = itemDto.Likes ?? default,
                Dislikes = itemDto.Dislikes ?? default
            };

            return result;
        }

        public static reviewDto.CreateCustomerReviewRequest ToApiModel(this CreateCustomerReviewRequest request)
        {
            var result = new reviewDto.CreateCustomerReviewRequest
            {
                AuthorNickname = request.AuthorNickname,
                Content = request.Content,
                ProductId = request.ProductId,
                Raiting = request.Raiting,
            };

            return result;
        }

        public static reviewDto.CustomerReviewSearchCriteria ToSearchCriteriaDto(this CustomerReviewSearchCriteria criteria)
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
