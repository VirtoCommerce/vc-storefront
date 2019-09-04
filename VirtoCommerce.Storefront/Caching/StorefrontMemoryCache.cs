using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;

namespace VirtoCommerce.Storefront.Caching
{
    public class StorefrontMemoryCache : IStorefrontMemoryCache
    {
        private readonly StorefrontOptions _storefrontOptions;
        private readonly IMemoryCache _memoryCache;
        private bool _disposed;

        public StorefrontMemoryCache(IMemoryCache memoryCache, IOptions<StorefrontOptions> storefrontOptions)
        {
            _memoryCache = memoryCache;
            _storefrontOptions = storefrontOptions.Value;
        }

        public MemoryCacheEntryOptions GetDefaultCacheEntryOptions()
        {
            var result = new MemoryCacheEntryOptions();

            if (!CacheEnabled)
            {
                result.AbsoluteExpirationRelativeToNow = TimeSpan.FromTicks(1);
            }
            else
            {
                if (AbsoluteExpiration != null)
                {
                    result.AbsoluteExpirationRelativeToNow = AbsoluteExpiration;
                }
                else if (SlidingExpiration != null)
                {
                    result.SlidingExpiration = SlidingExpiration;
                }
            }
            return result;
        }

        public ICacheEntry CreateEntry(object key)
        {
            var result = _memoryCache.CreateEntry(key);
            if (result != null)
            {
                var options = GetDefaultCacheEntryOptions();
                result.SetOptions(options);
            }
            return result;
        }

        public void Remove(object key)
        {
            _memoryCache.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        protected TimeSpan? AbsoluteExpiration => _storefrontOptions.CacheAbsoluteExpiration;
        protected TimeSpan? SlidingExpiration => _storefrontOptions.CacheSlidingExpiration;

        protected bool CacheEnabled => _storefrontOptions.CacheEnabled;

        /// <summary>
        /// Cleans up the background collection events.
        /// </summary>
        ~StorefrontMemoryCache()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }

                _disposed = true;
            }
        }
    }
}
