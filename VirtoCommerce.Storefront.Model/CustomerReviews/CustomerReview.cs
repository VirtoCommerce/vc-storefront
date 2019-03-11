using System;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.CustomerReviews
{
    public class CustomerReview : Entity
    {
        public string AuthorNickname { get; set; }
        public string Content { get; set; }
        public string ProductId { get; set; }
        public bool IsActive { get; set; }
        public bool IsRecommended { get; set; }
        public int VoteCount { get; set; }
        public int Rate { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
