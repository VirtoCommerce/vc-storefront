using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.Caching;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    /// <summary>
    /// https://www.jerriepelser.com/blog/creating-dynamic-authorization-policies-aspnet-core/
    /// </summary>
    public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly ISecurity _platformSecurityApi;
        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, ISecurity platformSecurityApi, IStorefrontMemoryCache memoryCache)
            : base(options)
        {
            _memoryCache = memoryCache;
            _platformSecurityApi = platformSecurityApi;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // Check static policies first
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                var map = await GetDynamicAuthorizationPoliciesFromPlatformPermissions();
                map.TryGetValue(policyName, out policy);
            }

            return policy;
        }

        private async Task<Dictionary<string, AuthorizationPolicy>> GetDynamicAuthorizationPoliciesFromPlatformPermissions()
        {
            var cacheKey = CacheKey.With(GetType(), "GetDynamicAuthorizationPoliciesFromPlatformPermissions");
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken());

                var resultLookup = new Dictionary<string, AuthorizationPolicy>();
                foreach (var permission in await _platformSecurityApi.GetPermissionsAsync())
                {
                    resultLookup[permission.Id] = new AuthorizationPolicyBuilder().AddRequirements(new PermissionAuthorizationRequirement { Permission = permission.Id }).Build();
                }

                //Register storefront permissions
                foreach (var permission in SecurityConstants.Permissions.AllPermissions)
                {
                    resultLookup[permission] = new AuthorizationPolicyBuilder().AddRequirements(new PermissionAuthorizationRequirement { Permission = permission }).Build();
                }
              
                return resultLookup;
            });
            return result;
        }
    }
}
