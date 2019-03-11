using System;
using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class CustomerReview : Drop
    {
        public string AuthorNickname { get; set; }
        public string Content { get; set; }
        public string ProductId { get; set; }
        public bool IsActive { get; set; }
        public bool IsRecommended { get; set; }
        public int VoteCount { get; set; }
        public int Rate { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
