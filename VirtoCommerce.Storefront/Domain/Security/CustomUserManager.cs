using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Security;
using platformSecurityDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using System.Threading;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CustomUserManager : AspNetUserManager<User>
    {
        private readonly IStoreModule _storeApi;
        private readonly IStorefrontSecurity _commerceCoreApi;
        private readonly ISecurity _platformSecurityApi;

        private readonly IMemoryCache _memoryCache;
        public CustomUserManager(IUserStore<User> userStore, IStoreModule storeApi, IStorefrontSecurity commerceCoreApi, ISecurity platformSecurityApi, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher,
                                IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators,
                                ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger, IMemoryCache memoryCache)
            : base(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _storeApi = storeApi;
            _commerceCoreApi = commerceCoreApi;
            _platformSecurityApi = platformSecurityApi;
            _memoryCache = memoryCache;
        }
            
        public override async Task<User> FindByIdAsync(string userId)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByIdAsync", userId);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _commerceCoreApi.GetUserByIdAsync(userId);
                if (userDto != null)
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                    return userDto.ToUser();
                }
                return null;
            }, cacheNullValue: false);
        }

        public override async Task<User> FindByNameAsync(string userName)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByNameAsync", userName);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _commerceCoreApi.GetUserByNameAsync(userName);
                if (userDto != null)
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                    return userDto.ToUser();
                }
                return null;
            }, cacheNullValue: false);

        }
      
        public override async Task<User> FindByEmailAsync(string email)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByEmailAsync", email);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _commerceCoreApi.GetUserByEmailAsync(email);
                if (userDto != null)
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                    return userDto.ToUser();
                }
                return null;
            }, cacheNullValue: false);
        }

        public override async Task<User> FindByLoginAsync(string loginProvider, string providerKey)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByLoginAsync", loginProvider, providerKey);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _commerceCoreApi.GetUserByLoginAsync(loginProvider, providerKey);
                if (userDto != null)
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                    return userDto.ToUser();
                }
                return null;
            }, cacheNullValue: false);
        }

        public override async Task<IdentityResult> CreateAsync(User user)
        {
            var dtoUser = user.ToUserDto();
            var resultDto = await _commerceCoreApi.CreateAsync(dtoUser);          
            return resultDto.ToIdentityResult();
        }

        public override async Task<IdentityResult> CreateAsync(User user, string password)
        {
            user.Password = password;
            return await CreateAsync(user);
        }

        protected override async Task<PasswordVerificationResult> VerifyPasswordAsync(IUserPasswordStore<User> store, User user, string password)
        {
            var result = await _commerceCoreApi.PasswordSignInAsync(user.UserName, password);
            return result.Status.EqualsInvariant("success") ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
       
        public override async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            var changePassword = new platformSecurityDto.ChangePasswordInfo
            {
                OldPassword = currentPassword,
                NewPassword = newPassword,
            };
            var resultDto = await _platformSecurityApi.ChangePasswordAsync(user.UserName, changePassword);
            return resultDto.ToIdentityResult();

        }

        public override async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            var resultDto = await _commerceCoreApi.ResetPasswordAsync(user.Id, token, newPassword);
            return resultDto.ToIdentityResult();
        }

        public override async Task<IdentityResult> AddLoginAsync(User user, UserLoginInfo login)
        {
            var updateUser = await FindByIdAsync(user.Id);

            if (updateUser != null)
            {
                updateUser.ExternalLogins.Add(new ExternalUserLoginInfo
                {
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey,
                    ProviderDisplayName = login.ProviderDisplayName
                });
            }

            var resultDto = await _platformSecurityApi.UpdateAsyncAsync(updateUser.ToPlatformUserDto());
            //Evict user from the cache
            SecurityCacheRegion.ExpireUser(user.Id);

            return resultDto.ToIdentityResult();
        }
    }

    //Stub for UserManager
    public class UserStoreStub : IUserStore<User>, IUserPasswordStore<User>
    {
        public Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }        

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
            return Task.CompletedTask; 
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }


}
