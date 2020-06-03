using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Caching;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Middleware
{
    public class CreateStorefrontRolesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISecurity _platformSecurityApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly ILogger _looger;
        public CreateStorefrontRolesMiddleware(RequestDelegate next, ISecurity platformSecurityApi, IStorefrontMemoryCache memoryCache, ILogger<CreateStorefrontRolesMiddleware> logger)
        {
            _next = next;
            _memoryCache = memoryCache;
            _platformSecurityApi = platformSecurityApi;
            _looger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var cacheKey = CacheKey.With(GetType(), "GetAllPlatformRoles");
            try
            {
                await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken());
                    var allRolesIds = (await _platformSecurityApi.SearchRolesAsync(new RoleSearchCriteria { Take = int.MaxValue })).Roles.Select(x => x.Id).ToArray();
                    foreach (var role in SecurityConstants.Roles.AllRoles)
                    {
                        if (!allRolesIds.Contains(role.Id))
                        {
                            await _platformSecurityApi.UpdateRoleAsync(role.ToRoleDto());
                        }
                    }
                    return allRolesIds;
                }, cacheNullValue: false);
            }
            catch (Exception ex)
            {
                _looger.LogError(ex, ex.Message);
            }

            await _next(context);
        }
    }

}
