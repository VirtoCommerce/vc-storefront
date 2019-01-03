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

        protected override string GetKey(Article article)
        {
            return article.Handle;
        }


    }
}
