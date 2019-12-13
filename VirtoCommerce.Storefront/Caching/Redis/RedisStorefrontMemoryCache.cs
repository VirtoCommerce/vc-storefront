using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using StackExchange.Redis;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Caching.Redis
{
    public class RedisStorefrontMemoryCache : StorefrontMemoryCache
    {
        private readonly ISubscriber _bus;
        private readonly RedisCachingOptions _redisCachingOptions;
        private readonly ILogger _log;
        private readonly RetryPolicy _retryPolicy;

        private static readonly string _cacheId = Guid.NewGuid().ToString("N");

        public RedisStorefrontMemoryCache(IMemoryCache memoryCache, IOptions<StorefrontOptions> options
            , ISubscriber bus
            , IOptions<RedisCachingOptions> redisCachingOptions
            , ILoggerFactory loggerFactory
            ) : base(memoryCache, options, loggerFactory)
        {
            _log = loggerFactory?.CreateLogger<RedisStorefrontMemoryCache>();
            _bus = bus;

            _redisCachingOptions = redisCachingOptions.Value;
            _bus.Subscribe(_redisCachingOptions.ChannelName, OnMessage);

            _retryPolicy = Policy.Handle<Exception>().WaitAndRetry(
                _redisCachingOptions.BusRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1)));
        }


        private void OnMessage(RedisChannel channel, RedisValue redisValue)
        {
            var message = JsonConvert.DeserializeObject<RedisCachingMessage>(redisValue);

            if (!string.IsNullOrEmpty(message.Id) && !message.Id.EqualsInvariant(_cacheId))
            {
                foreach (var item in message.CacheKeys)
                {
                    base.Remove(item);

                    _log.LogInformation($"remove local cache that cache key is {item}");
                }
            }
        }

        protected override void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            _log.LogInformation($"channel[{_redisCachingOptions.ChannelName }]: sending a message with key:{key} from instance:{ _cacheId } to all subscribers");

            var message = new RedisCachingMessage { Id = _cacheId, CacheKeys = new[] { key } };
            _retryPolicy.Execute(() => _bus.Publish(_redisCachingOptions.ChannelName, JsonConvert.SerializeObject(message)));

            base.EvictionCallback(key, value, reason, state);
        }
    }
}
