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
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CustomUserManager : AspNetUserManager<User>
    {
        public CustomUserManager(IUserStore<User> userStore, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher,
                                IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators,
                                ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger)
            : base(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }

    //Stub for UserManager
    public sealed class UserStoreStub : IUserStore<User>, IUserEmailStore<User>, IUserPasswordStore<User>, IUserLockoutStore<User>, IUserLoginStore<User>, IUserSecurityStampStore<User>
    {
        private readonly ISecurity _platformSecurityApi;
        private readonly IMemoryCache _memoryCache;
        private readonly IMemberService _memberService;
        public UserStoreStub(ISecurity platformSecurityApi, IMemberService memberService, IMemoryCache memoryCache)
        {
            _platformSecurityApi = platformSecurityApi;
            _memoryCache = memoryCache;
            _memberService = memberService;
        }

        #region IUserStore<User> members
        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            if(user.Contact != null)
            {
                user.Contact = await _memberService.CreateContactAsync(user.Contact);
            }
            var dtoUser = user.ToUserDto();
            var resultDto = await _platformSecurityApi.CreateAsyncAsync(dtoUser);
            return resultDto.ToIdentityResult();
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            await _platformSecurityApi.DeleteAsyncAsync(new[] { user.UserName });
            return IdentityResult.Success;
        }
        
        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByIdAsync", userId);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
           {
               var userDto = await _platformSecurityApi.GetUserByIdAsync(userId);
               if (userDto != null)
               {
                   cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                   return userDto.ToUser();
               }
               return null;
           }, cacheNullValue: false);

            //Load user associated contact
            if (result != null && result.ContactId != null)
            {
                result.Contact = await _memberService.GetContactByIdAsync(result.ContactId);
            }
            return result;
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByNameAsync", normalizedUserName);
            var result =  await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _platformSecurityApi.GetUserByNameAsync(normalizedUserName);
                if (userDto != null)
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                    return userDto.ToUser();
                }
                return null;
            }, cacheNullValue: false);

            //Load user associated contact
            if (result != null && result.ContactId != null)
            {
                result.Contact = await _memberService.GetContactByIdAsync(result.ContactId);
            }
            return result;
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
            if (user.Contact != null)
            {
                if (user.Contact.IsTransient())
                {
                    user.Contact = await _memberService.CreateContactAsync(user.Contact);
                }
                else
                {
                    await _memberService.UpdateContactAsync(user.Contact);
                }
            }
            
            var dtoUser = user.ToUserDto();
            var resultDto = await _platformSecurityApi.UpdateAsyncAsync(dtoUser);

            //Evict user from the cache
            SecurityCacheRegion.ExpireUser(user.Id);
            return resultDto.ToIdentityResult();
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
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _platformSecurityApi.GetUserByEmailAsync(normalizedEmail);
                if (userDto != null)
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                    return userDto.ToUser();
                }
                return null;
            }, cacheNullValue: false);

            //Load user associated contact
            if (result != null && result.ContactId != null)
            {
                result.Contact = await _memberService.GetContactByIdAsync(result.ContactId);
            }

            return result;
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
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _platformSecurityApi.GetUserByLoginAsync(loginProvider, providerKey);
                if (userDto != null)
                {
                    cacheEntry.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));
                    return userDto.ToUser();
                }
                return null;
            }, cacheNullValue: false);


            //Load user associated contact
            if (result != null && result.ContactId != null)
            {
                result.Contact = await _memberService.GetContactByIdAsync(result.ContactId);
            }
            return result;
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
            return Task.FromResult(user.PasswordHash);
        }
        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }
        #endregion

        #region IUserSecurityStampStore<User> members
        public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        } 
        #endregion

        public void Dispose()
        {
            // Cleanup
        }

     
    }


}
