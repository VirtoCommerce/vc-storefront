using System;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.CustomerReviews
{
    /// <summary>
    /// Customer review model
    /// </summary>
    public partial class CustomerReview : Entity
    {
        /// <summary>
        /// Nickname of review owner
        /// </summary>
        public string AuthorNickname { get; set; }

        /// <summary>
        /// Text of review
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// State of review by activity
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Id of reviewed product
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Date when review created
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Last date when review was changed
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Nickname of review creator
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Nickname of last review changer
        /// </summary>
        public string ModifiedBy { get; set; }
    }
}
