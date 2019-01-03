using System.Collections.Generic;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public class Collections : ItemCollection<Collection>
    {
        public Collections(IEnumerable<Collection> collections)
            : base(collections)
        {
        }

        protected override string GetKey(Collection obj)
        {
            return obj.Handle;
        }


    }
}
