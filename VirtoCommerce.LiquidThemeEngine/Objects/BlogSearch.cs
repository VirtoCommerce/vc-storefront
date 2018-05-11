using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    [DataContract]
    public partial class BlogSearch : Drop
    {
        [DataMember]
        public string Category { get; set; }
        [DataMember]
        public string Tag { get; set; }
        [DataMember]
        public string Author { get; set; }
    }
}
