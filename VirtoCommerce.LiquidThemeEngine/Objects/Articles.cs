using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class Articles : ItemCollection<Article>
    {
        public Articles(IEnumerable<Article> articles)
            : base(articles)
        {
        }

        public override object BeforeMethod(string method)
        {
            return this.SingleOrDefault(x => x.Handle.Equals(method, StringComparison.OrdinalIgnoreCase));
        }

        public override bool Contains(object value)
        {
            return false;
        }
    }
}