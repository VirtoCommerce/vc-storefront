using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class ListExtensions
    {
        public static List<T> AddIf<T>(this List<T> source, bool condition, T obj)
        {
            if (!condition)
                return source;
            source.Add(obj);
            return source;
        }
    }
}
