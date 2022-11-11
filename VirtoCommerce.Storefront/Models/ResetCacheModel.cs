using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Models
{
    /// <summary>
    /// Cache Event Model For Web Hook from Virto Commmerce Platform
    /// </summary>
    public class ResetCacheEventModel
    {
        public string EventId { get; set; }

        public ResetCacheEventBodyModel[] EventBody { get; set; }
    }

    public class ResetCacheEventBodyModel
    {
        public string ObjectType { get; set; }

        public string Id { get; set; }

        public string Path { get; set; }

        public string Type { get; set; }
    }
}
