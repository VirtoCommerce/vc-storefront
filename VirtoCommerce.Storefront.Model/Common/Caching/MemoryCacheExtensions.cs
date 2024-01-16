using System;
using System.Threading.Tasks;
using AsyncKeyedLock;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.Model.Caching;

namespace VirtoCommerce.Storefront.Model.Common.Caching
{
    public static class MemoryCacheExtensions
    {
        private static readonly AsyncKeyedLocker<string> _locker = new(o =>
        {
            o.PoolSize = 20;
            o.PoolInitialFill = 1;
        });
        public static async Task<TItem> GetOrCreateExclusiveAsync<TItem>(this IMemoryCache cache, string key, Func<MemoryCacheEntryOptions, Task<TItem>> factory, bool cacheNullValue = true)
        {
            if (!cache.TryGetValue(key, out var result))
            {
                using (await _locker.LockAsync(key).ConfigureAwait(false))
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
                using (_locker.Lock(key))
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
