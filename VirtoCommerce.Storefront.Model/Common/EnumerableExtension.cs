using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Common
{
    public static class EnumerableExtensions
    {
        /// <summary>Indicates whether the specified enumerable is null or has a length of zero.</summary>
        /// <param name="data">The data to test.</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> data)
        {
            return data == null || !data.Any();
        }

        public static int GetOrderIndependentHashCode<T>(this IEnumerable<T> source)
        {
            int hash = 0;
            //Need to force order to get  order independent hash code
            foreach (T element in source.OrderBy(x => x, Comparer<T>.Default))
            {
                hash = hash ^ EqualityComparer<T>.Default.GetHashCode(element);
            }
            return hash;
        }

        /// <summary>
        /// Performs the indicated action on each item.
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        /// <remarks>If an exception occurs, the action will not be performed on the remaining items.</remarks>
        public static void Apply<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// Performs the indicated action on each item. Boxing free for <c>List+Enumerator{T}</c>.
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        /// <remarks>If an exception occurs, the action will not be performed on the remaining items.</remarks>
        public static void Apply<T>(this List<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}
