using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Linklists : ItemCollection<Linklist>
    {
        public Linklists(IEnumerable<Linklist> linklists)
            : base(linklists)
        {
        }

        protected override string GetKey(Linklist obj)
        {
            return obj.Handle;
        }

    }
}
