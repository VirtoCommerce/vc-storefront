using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class Pages : ItemCollection<Page>
    {
        public Pages(IEnumerable<Page> pages)
            : base(pages)
        {
        }

        protected override string GetKey(Page page)
        {
            return page.Handle;
        }

    }
}
