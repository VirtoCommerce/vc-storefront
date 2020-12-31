using System;
using Microsoft.Extensions.Primitives;

namespace VirtoCommerce.Storefront.Model.Common.Caching
{
    /// <summary>
    /// Represents strongly typed cache region containing cancellation token for a concrete cache region type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CancellableCacheRegion<T>
    {
        private static readonly string _regionName = typeof(T).Name;

        protected CancellableCacheRegion()
        {
        }

        public static IChangeToken CreateChangeTokenForKey(string key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return new CompositeChangeToken(new[] { CreateChangeToken(), CacheCancellableTokensRegistry.CreateChangeToken(GenerateRegionTokenKey(key)) });
        }

        public static IChangeToken CreateChangeToken()
        {
            return CacheCancellableTokensRegistry.CreateChangeToken(GenerateRegionTokenKey());
        }

        public static void ExpireTokenForKey(string key)
        {
            if (!(key is null))
            {
                CacheCancellableTokensRegistry.TryCancelToken(GenerateRegionTokenKey(key));
            }
        }

        public static void ExpireRegion()
        {
            CacheCancellableTokensRegistry.TryCancelToken(GenerateRegionTokenKey());
        }

        private static string GenerateRegionTokenKey(string key = null)
        {
            if (!(key is null))
            {
                return $"{_regionName}:{key}";
            }
            return $"{_regionName}";
        }

    }
}
