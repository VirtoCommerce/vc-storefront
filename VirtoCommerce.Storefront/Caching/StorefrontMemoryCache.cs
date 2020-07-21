using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Caching
{
    public class StorefrontMemoryCache : IStorefrontMemoryCache
    {
        private readonly StorefrontOptions _storefrontOptions;
        private readonly IMemoryCache _memoryCache;
        private bool _disposed;
        private readonly ILogger _log;
        private readonly IWorkContextAccessor _workContextAccessor;

        public StorefrontMemoryCache(IMemoryCache memoryCache, IOptions<StorefrontOptions> storefrontOptions, ILoggerFactory loggerFactory, IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
            _memoryCache = memoryCache;
            _storefrontOptions = storefrontOptions.Value;
            CacheEnabled = _storefrontOptions.CacheEnabled;
            _log = loggerFactory?.CreateLogger<StorefrontMemoryCache>();
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

                result.AddExpirationToken(GlobalCacheRegion.CreateChangeToken());
            }
            return result;
        }

        public virtual ICacheEntry CreateEntry(object key)
        {
            var result = _memoryCache.CreateEntry(key);
            if (result != null)
            {
                result.RegisterPostEvictionCallback(callback: EvictionCallback);
                var options = GetDefaultCacheEntryOptions();
                result.SetOptions(options);
            }
            return result;
        }

        public virtual void Remove(object key)
        {
            _memoryCache.Remove(key);
        }

        public virtual bool TryGetValue(object key, out object value)
        {
            var result = _memoryCache.TryGetValue(key, out value);
            //Do not use value from cache for preview mode
            if(_workContextAccessor.WorkContext != null && _workContextAccessor.WorkContext.IsPreviewMode)
            {
                result = false;
            }
            return result;
        }

        protected TimeSpan? AbsoluteExpiration => _storefrontOptions.CacheAbsoluteExpiration;
        protected TimeSpan? SlidingExpiration => _storefrontOptions.CacheSlidingExpiration;

        protected bool CacheEnabled { get; set; }


        ~StorefrontMemoryCache()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _memoryCache.Dispose();
                }
                _disposed = true;
            }
        }

        protected virtual void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            _log.LogInformation($"EvictionCallback: Cache with key {key} has expired.");
        }
    }
}
