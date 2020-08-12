using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Caching.Redis
{
    internal class RedisStorefrontMemoryCache : StorefrontMemoryCache
    {
        public static string ServerId { get; } = $"{Environment.MachineName}_{Guid.NewGuid():N}";

        private readonly ISubscriber _bus;
        private readonly RedisCachingOptions _redisCachingOptions;
        private readonly StorefrontOptions _storefrontOptions;
        private readonly IConnectionMultiplexer _connection;

        private bool _disposed;

        public RedisStorefrontMemoryCache(IMemoryCache memoryCache
            , IOptions<StorefrontOptions> cachingOptions
            , IOptions<RedisCachingOptions> redisCachingOptions
            , IWorkContextAccessor workContextAccessor
            , ILoggerFactory loggerFactory
            ) : base(memoryCache, cachingOptions, loggerFactory, workContextAccessor)
        {
            _connection = ConnectionMultiplexer.Connect(redisCachingOptions.Value.Configuration);

            _redisCachingOptions = redisCachingOptions.Value;
            _storefrontOptions = cachingOptions.Value;

            _connection.ConnectionFailed += OnConnectionFailed;
            _connection.ConnectionRestored += OnConnectionRestored;

            _bus = _connection.GetSubscriber();
            _bus.Subscribe(_redisCachingOptions.ChannelName, OnMessage, CommandFlags.FireAndForget);
        }

        protected virtual void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            // If we have no connection to Redis, we can't invalidate cache on another platform instances,
            // so the better idea is to disable cache at all for data consistence
            CacheEnabled = false;
            // We should fully clear cache because we don't know
            // what's changed until platform found Redis is unavailable
            GlobalCacheRegion.ExpireRegion();
        }

        protected virtual void OnConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            // Return cache to the same state as it was initially.
            // Don't set directly true because it may be disabled in app settings
            CacheEnabled = _storefrontOptions.CacheEnabled;
            // We should fully clear cache because we don't know
            // what's changed in another instances since Redis became unavailable
            GlobalCacheRegion.ExpireRegion();
        }


        protected virtual void OnMessage(RedisChannel channel, RedisValue redisValue)
        {
            var message = JsonConvert.DeserializeObject<RedisCachingMessage>(redisValue);

            if (!string.IsNullOrEmpty(message.Id) && !message.Id.EqualsInvariant(ServerId))
            {
                foreach (var item in message.CacheKeys)
                {
                    base.Remove(item);
                }
            }
        }

        protected override void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            var message = new RedisCachingMessage { Id = ServerId, CacheKeys = new[] { key } };
            _bus.Publish(_redisCachingOptions.ChannelName, JsonConvert.SerializeObject(message), CommandFlags.FireAndForget);

            base.EvictionCallback(key, value, reason, state);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _bus.Unsubscribe(_redisCachingOptions.ChannelName, null, CommandFlags.FireAndForget);
                    _connection.ConnectionFailed -= OnConnectionFailed;
                    _connection.ConnectionRestored -= OnConnectionRestored;
                }
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
