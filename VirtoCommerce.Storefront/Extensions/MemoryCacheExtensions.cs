using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class MemoryCacheExtensions
    {
        private static ConcurrentDictionary<string, object> _lockLookup = new ConcurrentDictionary<string, object>();
        public static async Task<TItem> GetOrCreateExclusiveAsync<TItem>(this IMemoryCache cache, string key, Func<ICacheEntry, Task<TItem>> factory, bool cacheNullValue = true)
        {
            using (await AsyncLock.GetLockByKey(key).LockAsync())
            {
                return await cache.GetOrCreateAsync(key, factory, cacheNullValue);
            }
        }

        public static TItem GetOrCreateExclusive<TItem>(this IMemoryCache cache, string key, Func<ICacheEntry, TItem> factory, bool cacheNullValue = true)
        {
            lock(_lockLookup.GetOrAdd(key, new object()))
            { 
                return cache.GetOrCreate(key, factory, cacheNullValue);
            }
        }

        public static async Task<TItem> GetOrCreateAsync<TItem>(this IMemoryCache cache, string key, Func<ICacheEntry, Task<TItem>> factory, bool cacheNullValue)
        {
            if (!cache.TryGetValue(key, out object result))
            {
                var entry = cache.CreateEntry(key);
                result = await factory(entry);
                if (result != null || cacheNullValue)
                {
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }
            }
            return (TItem)result;
        }

        public static TItem GetOrCreate<TItem>(this IMemoryCache cache, string key, Func<ICacheEntry, TItem> factory, bool cacheNullValue)
        {
            if (!cache.TryGetValue(key, out object result))
            {
                var entry = cache.CreateEntry(key);
                result = factory(entry);
                if (result != null || cacheNullValue)
                {
                    entry.SetValue(result);
                    // need to manually call dispose instead of having a using
                    // in case the factory passed in throws, in which case we
                    // do not want to add the entry to the cache
                    entry.Dispose();
                }
            }

            return (TItem)result;
        }
    }
}
