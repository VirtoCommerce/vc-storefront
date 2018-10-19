using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Specialized;
using System.Text.Encodings.Web;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class QueryCollectionExtensions
    {
        public static NameValueCollection ToNameValueCollection(this IQueryCollection queryCollection, HtmlEncoder htmlEncoder)
        {
            if (queryCollection == null)
            {
                throw new ArgumentNullException(nameof(queryCollection));
            }
            var result = new NameValueCollection();
            foreach (var pair in queryCollection)
            {
                result.Add(htmlEncoder.Encode(pair.Key), htmlEncoder.Encode(pair.Value));
            }
            return result;
        }
    }
}
