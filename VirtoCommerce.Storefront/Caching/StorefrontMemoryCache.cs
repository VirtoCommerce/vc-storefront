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
        private static TimeSpan? _absoluteExpiration;
        private static bool? _cacheEnabled;
        private bool _disposed;
        private static object _lockObject = new object();

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

        protected TimeSpan AbsoluteExpiration
        {
            get
            {
                if (_absoluteExpiration == null)
                {
                    lock (_lockObject)
                    {
                        if (_absoluteExpiration == null)
                        {
                            _absoluteExpiration = _settingManager.Value.CacheAbsoluteExpiration;
                        }
                    }
                }
                return _absoluteExpiration.Value;
            }
        }

        protected bool CacheEnabled
        {
            get
            {
                if (_cacheEnabled == null)
                {
                    lock (_lockObject)
                    {
                        if (_cacheEnabled == null)
                        {
                            _cacheEnabled = _settingManager.Value.CacheEnabled;
                        }
                    }
                }
                return _cacheEnabled.Value;
            }
        }


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
