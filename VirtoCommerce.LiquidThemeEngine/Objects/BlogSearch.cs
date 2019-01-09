using System.Runtime.Serialization;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    [DataContract]
    public partial class BlogSearch : ValueObject
    {
        [DataMember]
        public string Category { get; set; }
        [DataMember]
        public string Tag { get; set; }
        [DataMember]
        public string Author { get; set; }
    }
}
