using System;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using VirtoCommerce.Storefront.Caching.Redis;
using VirtoCommerce.Storefront.Model.Caching;

namespace VirtoCommerce.Storefront.Caching
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStorefrontCache(this IServiceCollection services, string redisConnectionString, Action<RedisCachingOptions> configure)
        {
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.Configure<RedisCachingOptions>(o =>
                {
                    o.Configuration = ConfigurationOptions.Parse(redisConnectionString);
                    configure(o);
                });
                services.AddSingleton<IStorefrontMemoryCache, RedisStorefrontMemoryCache>();
            }
            else
            {
                services.AddSingleton<IStorefrontMemoryCache, StorefrontMemoryCache>();
            }

            return services;
        }

    }
}
