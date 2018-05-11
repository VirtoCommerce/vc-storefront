using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Descriptions : ItemCollection<Description>
    {
        public Descriptions(IEnumerable<Description> descriptions)
            : base(descriptions)
        {
        }

        public override object BeforeMethod(string method)
        {
            return this.SingleOrDefault(x => x.Type.Equals(method, StringComparison.OrdinalIgnoreCase));
        }
    }
}