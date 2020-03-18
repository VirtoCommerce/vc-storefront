using System;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class QueryCollectionExtensions
    {
        public static NameValueCollection ToNameValueCollection(this IQueryCollection queryCollection)
        {
            if (queryCollection == null)
            {
                throw new ArgumentNullException(nameof(queryCollection));
            }
            var result = new NameValueCollection();
            foreach (var pair in queryCollection)
            {
                result.Add(pair.Key, pair.Value);
            }
            return result;
        }
    }
}
