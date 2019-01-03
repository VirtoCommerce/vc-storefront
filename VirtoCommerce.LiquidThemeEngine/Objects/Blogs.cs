using System.Collections.Generic;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class Blogs : ItemCollection<Blog>
    {
        public Blogs(IEnumerable<Blog> blogs)
            : base(blogs)
        {
        }

        protected override string GetKey(Blog blog)
        {
            return blog.Handle;
        }

    }
}
