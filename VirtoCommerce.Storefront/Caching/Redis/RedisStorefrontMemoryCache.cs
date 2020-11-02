using System;
using System.Linq;
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
        private static string _instanceId { get; } = $"{Environment.MachineName}_{Guid.NewGuid():N}";

        private readonly ISubscriber _bus;
        private readonly RedisCachingOptions _redisCachingOptions;
        private readonly StorefrontOptions _storefrontOptions;
        private readonly IConnectionMultiplexer _connection;
        private readonly ILogger<RedisStorefrontMemoryCache> _log;

        private bool _isSubscribed;
        private bool _disposed;
        private readonly object _lock = new object();

        public RedisStorefrontMemoryCache(IMemoryCache memoryCache
            , IOptions<StorefrontOptions> cachingOptions
            , IOptions<RedisCachingOptions> redisCachingOptions
            , IWorkContextAccessor workContextAccessor
            , ILoggerFactory loggerFactory
            ) : base(memoryCache, cachingOptions, loggerFactory, workContextAccessor)
        {
            _connection = ConnectionMultiplexer.Connect(redisCachingOptions.Value.Configuration);
            _log = loggerFactory?.CreateLogger<RedisStorefrontMemoryCache>();
            _bus = _connection.GetSubscriber();

            _redisCachingOptions = redisCachingOptions.Value;
            _storefrontOptions = cachingOptions.Value;

            CacheCancellableTokensRegistry.OnTokenCancelled = CacheCancellableTokensRegistry_OnTokenCancelled;
        }

        private void CacheCancellableTokensRegistry_OnTokenCancelled(TokenCancelledEventArgs e)
        {
            var message = new RedisCachingMessage { InstanceId = _instanceId, IsToken = true, CacheKeys = new[] { e.TokenKey } };
            Publish(message);
            _log.LogTrace($"Published token cancellation message {message}");
        }

        protected virtual void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _log.LogError($"Redis disconnected from instance { _instanceId }. Endpoint is {e.EndPoint}, failure type is {e.FailureType}");

            // If we have no connection to Redis, we can't invalidate cache on another platform instances,
            // so the better idea is to disable cache at all for data consistence
            CacheEnabled = false;
            // We should fully clear cache because we don't know
            // what's changed until platform found Redis is unavailable
            GlobalCacheRegion.ExpireRegion();
        }

        protected virtual void OnConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _log.LogTrace($"Redis backplane connection restored for instance { _instanceId }");

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

            if (!string.IsNullOrEmpty(message.InstanceId) && !message.InstanceId.EqualsInvariant(_instanceId))
            {
                _log.LogTrace($"Received message {message}");

                foreach (var key in message.CacheKeys?.OfType<string>() ?? Array.Empty<string>())
                {
                    if (message.IsToken)
                    {
                        _log.LogTrace($"Trying to cancel token with key: {key}");
                        CacheCancellableTokensRegistry.TryCancelToken(key, raiseEvent: false);
                    }
                    else
                    {
                        _log.LogTrace($"Trying to remove cache entry with key: {key} from in-memory cache");
                        base.Remove(key);
                    }
                }
            }
        }

        public override bool TryGetValue(object key, out object value)
        {
            //We can't do subscription in the ctor due to the fact that it can be called multiple times despite the fact that it registered as a singleton.
            //So we have delayed the connection and subscription to the Redis server until the first cache call.
            EnsureRedisServerConnection();
            return base.TryGetValue(key, out value);
        }

        protected override void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            var message = new RedisCachingMessage { InstanceId = _instanceId, CacheKeys = new[] { key } };
            Publish(message);
            _log.LogTrace($"Published message {message} to the Redis backplane");

            base.EvictionCallback(key, value, reason, state);
        }

        private void Publish(RedisCachingMessage message)
        {
            EnsureRedisServerConnection();
            _bus.Publish(_redisCachingOptions.ChannelName, JsonConvert.SerializeObject(message), CommandFlags.FireAndForget);
        }

        private void EnsureRedisServerConnection()
        {
            if (!_isSubscribed)
            {
                lock (_lock)
                {
                    if (!_isSubscribed)
                    {
                        _connection.ConnectionFailed += OnConnectionFailed;
                        _connection.ConnectionRestored += OnConnectionRestored;

                        _bus.Subscribe(_redisCachingOptions.ChannelName, OnMessage, CommandFlags.FireAndForget);

                        _log.LogTrace($"Successfully subscribed to Redis backplane channel {_redisCachingOptions.ChannelName } with instance id:{ _instanceId }");
                        _isSubscribed = true;
                    }
                }
            }

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
