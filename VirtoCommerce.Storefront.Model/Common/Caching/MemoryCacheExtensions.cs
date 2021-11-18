using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.Model.Caching;

namespace VirtoCommerce.Storefront.Model.Common.Caching
{
    public static class MemoryCacheExtensions
    {
        private static readonly ConcurrentDictionary<string, object> _lockLookup = new ConcurrentDictionary<string, object>();
        public static async Task<TItem> GetOrCreateExclusiveAsync<TItem>(this IMemoryCache cache, string key, Func<MemoryCacheEntryOptions, Task<TItem>> factory, bool cacheNullValue = true)
        {
            if (!cache.TryGetValue(key, out var result))
            {
                using (await AsyncLock.GetLockByKey(key).LockAsync())
                {
                    if (!cache.TryGetValue(key, out result))
                    {
                        var options = cache is IStorefrontMemoryCache storefrontMemoryCache ? storefrontMemoryCache.GetDefaultCacheEntryOptions() : new MemoryCacheEntryOptions();
                        result = await factory(options);
                        if (result != null || cacheNullValue)
                        {
                            cache.Set(key, result, options);
                        }
                    }
                }
            }
            return (TItem)result;
        }

        public static TItem GetOrCreateExclusive<TItem>(this IMemoryCache cache, string key, Func<MemoryCacheEntryOptions, TItem> factory, bool cacheNullValue = true)
        {
            if (!cache.TryGetValue(key, out var result))
            {
                lock (_lockLookup.GetOrAdd(key, new object()))
                {
                    if (!cache.TryGetValue(key, out result))
                    {
                        var options = cache is IStorefrontMemoryCache storefrontMemoryCache ? storefrontMemoryCache.GetDefaultCacheEntryOptions() : new MemoryCacheEntryOptions();
                        result = factory(options);
                        if (result != null || cacheNullValue)
                        {
                            cache.Set(key, result, options);
                        }
                    }
                }
            }
            return (TItem)result;
        }
    }
}
