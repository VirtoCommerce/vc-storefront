using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;

namespace VirtoCommerce.Storefront.Caching
{
    public class StorefrontMemoryCache : IStorefrontMemoryCache
    {
        private readonly IOptions<StorefrontOptions> _settingManager;
        private readonly IMemoryCache _memoryCache;
        private bool _disposed;

        public StorefrontMemoryCache(IMemoryCache memoryCache, IOptions<StorefrontOptions> settingManager)
        {
            _memoryCache = memoryCache;
            _settingManager = settingManager;
        }

        public ICacheEntry CreateEntry(object key)
        {
            var result = _memoryCache.CreateEntry(key);
            if (result != null)
            {
                var absoluteExpiration = CacheEnabled ? AbsoluteExpiration : TimeSpan.FromTicks(1);
                result.SetAbsoluteExpiration(absoluteExpiration);
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

        protected TimeSpan AbsoluteExpiration => _settingManager.Value.CacheAbsoluteExpiration;

        protected bool CacheEnabled => _settingManager.Value.CacheEnabled;

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
