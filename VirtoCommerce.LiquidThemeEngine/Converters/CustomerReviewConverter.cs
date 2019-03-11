using VirtoCommerce.LiquidThemeEngine.Objects;
using storefrontModel = VirtoCommerce.Storefront.Model.CustomerReviews;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class CustomerReviewStaticConverter
    {
        public static CustomerReview ToShopifyModel(this storefrontModel.CustomerReview item)
        {
            return new ShopifyModelConverter().ToLiquidCustomerReview(item);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual CustomerReview ToLiquidCustomerReview(storefrontModel.CustomerReview item)
        {
            return new CustomerReview
            {
                AuthorNickname = item.AuthorNickname,
                Content = item.Content,
                CreatedDate = item.CreatedDate,
                IsActive = item.IsActive,
                ProductId = item.ProductId,
                IsRecommended = item.IsRecommended,
                VoteCount = item.VoteCount,
                Rate = item.Rate
            };
        }
    }
}
