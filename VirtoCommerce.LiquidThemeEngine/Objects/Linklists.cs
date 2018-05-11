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

        public override object BeforeMethod(string method)
        {
            return this.FirstOrDefault(x => x.Handle.EqualsInvariant(method));
        }

        #region ItemCollection Members
        public override bool Contains(object value)
        {
            return false;
        }
        #endregion
    }
}