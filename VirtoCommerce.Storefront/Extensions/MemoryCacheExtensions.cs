using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class MemoryCacheExtensions
    {
        private static ConcurrentDictionary<string, object> _lockLookup = new ConcurrentDictionary<string, object>();
        public static async Task<TItem> GetOrCreateExclusiveAsync<TItem>(this IMemoryCache cache, string key, Func<ICacheEntry, Task<TItem>> factory, bool cacheNullValue = true)
        {
            if (!cache.TryGetValue(key, out object result))
            {
                using (await AsyncLock.GetLockByKey(key).LockAsync())
                {
                    if (!cache.TryGetValue(key, out result))
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
                }
            }
            return (TItem)result;
        }

        public static TItem GetOrCreateExclusive<TItem>(this IMemoryCache cache, string key, Func<ICacheEntry, TItem> factory, bool cacheNullValue = true)
        {
            if (!cache.TryGetValue(key, out object result))
            {
                lock (_lockLookup.GetOrAdd(key, new object()))
                {
                    if (!cache.TryGetValue(key, out result))
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
                }
            }
            return (TItem)result;
        }
    }
}
