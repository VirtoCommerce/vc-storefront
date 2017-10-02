using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{

    public class CustomUserStore :  IUserStore<User>,
                                    IUserEmailStore<User>,
                                    IUserLoginStore<User>,
                                    IUserPhoneNumberStore<User>,
                                    IUserTwoFactorStore<User>
    {
        private readonly IStorefrontSecurityService _securityService;
        private readonly IMemoryCache _memoryCache;

        public CustomUserStore(IStorefrontSecurityService securityService, IMemoryCache memoryCache)
        {
            _securityService = securityService;
            _memoryCache = memoryCache;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {            
            var result = await _securityService.CreateAsync(user);

            if (result.Succeeded == true)
            {
                //Load newly created account from API
                user = await _securityService.GetUserByNameAsync(user.UserName);
                user.IsRegisteredUser = true;             
                return IdentityResult.Success;
            }
            return IdentityResult.Failed();
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            //TODO:
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByIdAsync", userId);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(UserStoreCacheRegion.CreateChangeToken());
                return await _securityService.GetUserByIdAsync(userId);
         
            }, cacheNullValue: false);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByNameAsync", normalizedUserName);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(UserStoreCacheRegion.CreateChangeToken());
                return await _securityService.GetUserByNameAsync(normalizedUserName);
            }, cacheNullValue: false);

        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByEmailAsync", normalizedEmail);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(UserStoreCacheRegion.CreateChangeToken());
                return await _securityService.GetUserByEmailAsync(normalizedEmail);
            }, cacheNullValue: false);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserNameNormalized);
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

    

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }    

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;

            return Task.FromResult(true);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            // Do nothing. In this simple example, the normalized user name is generated from the user name.
            return Task.FromResult(true);
        }

        public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;

            return Task.FromResult(true);
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {
            // Just returning an empty list because I don't feel like implementing this. You should get the idea though...
            IList<UserLoginInfo> logins = new List<UserLoginInfo>();
            return Task.FromResult(logins);
        }

        public async Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByLoginAsync", loginProvider, providerKey);
            var user = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(UserStoreCacheRegion.CreateChangeToken());
                return await _securityService.GetUserByLoginAsync(loginProvider, providerKey);
            }, cacheNullValue: false);
            
            return null;
        }

        public async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {  
            await _securityService.CreateAsync(user);
        }

        public Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }

    }
}
