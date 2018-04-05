using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Security;
using platformSecurityDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;

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

        public override Task<IdentityResult> CreateAsync(User user, string password)
        {
            //Store password in user object to use later in IUserPasswordStore
            user.Password = password;
            return base.CreateAsync(user, password);
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

    }

    //Stub for UserManager
    public class UserStoreStub : IUserStore<User>, IUserEmailStore<User>, IUserPasswordStore<User>, IUserLockoutStore<User>, IUserLoginStore<User>
    {
        private readonly ISecurity _platformSecurityApi;
        private readonly IStorefrontSecurity _commerceCoreApi;
        private readonly IMemoryCache _memoryCache;
        public UserStoreStub(ISecurity platformSecurityApi, IStorefrontSecurity commerceCoreApi, IMemoryCache memoryCache)
        {
            _platformSecurityApi = platformSecurityApi;
            _commerceCoreApi = commerceCoreApi;
            _memoryCache = memoryCache;
        }

        #region IUserStore<User> members
        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var dtoUser = user.ToUserDto(await GetAllPlatformRolesAsync());
            var resultDto = await _commerceCoreApi.CreateAsync(dtoUser);
            return resultDto.ToIdentityResult();
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
                var userDto = await _commerceCoreApi.GetUserByIdAsync(userId);
                if (userDto != null)
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                    return userDto.ToUser();
                }
                return null;
            }, cacheNullValue: false);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByNameAsync", normalizedUserName);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _commerceCoreApi.GetUserByNameAsync(normalizedUserName);
                if (userDto != null)
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                    return userDto.ToUser();
                }
                return null;
            }, cacheNullValue: false);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var dtoUser = user.ToPlatformUserDto(await GetAllPlatformRolesAsync());
            var resultDto = await _platformSecurityApi.UpdateAsyncAsync(dtoUser);
            //Evict user from the cache
            SecurityCacheRegion.ExpireUser(user.Id);
            return resultDto.ToIdentityResult();
        }

        private async Task<IEnumerable<platformSecurityDto.Role>> GetAllPlatformRolesAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "GetAllPlatformRolesLookup");
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken());
                var result = await _platformSecurityApi.SearchRolesAsync(new platformSecurityDto.RoleSearchRequest { TakeCount = int.MaxValue });
                return result.Roles;
            });
        }
        #endregion

        #region IUserLockoutStore<User> members
        public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }
        
        public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult((DateTimeOffset?)user.LockoutEndDateUtc);
        }
        public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }
        
        public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }
        
        public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEndDateUtc = lockoutEnd?.UtcDateTime;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserEmailStore<User> members

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByEmailAsync", normalizedEmail);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _commerceCoreApi.GetUserByEmailAsync(normalizedEmail);
                if (userDto != null)
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                    return userDto.ToUser();
                }
                return null;
            }, cacheNullValue: false);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        #endregion

        #region IUserLoginStore<User> members
        public Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            user.ExternalLogins.Add(new ExternalUserLoginInfo
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName
            });
            return Task.CompletedTask;
        }

        public async Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
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

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {
            IList<UserLoginInfo> result = user.ExternalLogins?.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToList();
            return Task.FromResult(result);
        }

        public Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var existUserLogin = user.ExternalLogins?.FirstOrDefault(x => x.LoginProvider.EqualsInvariant(loginProvider) && x.ProviderKey.EqualsInvariant(providerKey));
            if(existUserLogin != null)
            {
                user.ExternalLogins.Remove(existUserLogin);
            }
            return Task.CompletedTask;
        }
        #endregion

        #region IUserPasswordStore<User> members
        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            //This method will never be called thanks to UserManager overrides
            throw new NotImplementedException();
        }
        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            //We do not allow users haven't  password
            return Task.FromResult(true);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }
        #endregion


        public void Dispose()
        {
        }

    }


}
