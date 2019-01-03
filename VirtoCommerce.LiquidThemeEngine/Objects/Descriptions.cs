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

        protected override string GetKey(Description obj)
        {
            return obj.Type;
        }

    }
}
