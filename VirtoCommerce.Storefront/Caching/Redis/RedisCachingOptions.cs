using System;
using StackExchange.Redis;

namespace VirtoCommerce.Storefront.Caching.Redis
{
    public class RedisCachingOptions
    {
        /// <summary>
        /// Gets or sets configuration options exposed by <c>StackExchange.Redis</c>.
        /// </summary>
        public ConfigurationOptions Configuration { get; set; } = new ConfigurationOptions
        {
            // Enable reconnecting by default
            AbortOnConnectFail = false
        };

        public string ChannelName { get; set; }

        [Obsolete("Use Redis connection string parameters for retry policy configration")]
        public int BusRetryCount { get; set; } = 3;
    }
}
