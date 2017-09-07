using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Recommendations
{
    /// <summary>
    /// Context used for evaluation recommendation for given  parameters 
    /// </summary>
    public partial class RecommendationEvalContext
    {
        public RecommendationEvalContext()
        {
            ProductIds = new List<string>();
            Take = 20;
        }

        /// <summary>
        /// Recommendation data provider (Cognitive, Association etc)
        /// </summary>
        public string Provider { get; set;}
        /// <summary>
        /// Type of requested recommendations may be unique for each kind of provider
        /// Coginitive - User2Item, Item2Item, FBT
        /// Association - Recommendations, UpSale, CrossSale etc
        /// </summary>
        public string Type { get; set; }

        public string StoreId { get; set; }

        public string UserId { get; set; }

        public ICollection<string> ProductIds { get; set; }

        public int Take { get; set; }           
      
    }
}
