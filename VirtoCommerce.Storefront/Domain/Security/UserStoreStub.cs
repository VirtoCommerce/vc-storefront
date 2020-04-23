using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    //Stub for UserManager
    public sealed class UserStoreStub :
        IUserEmailStore<User>,
        IUserPasswordStore<User>,
        IUserLockoutStore<User>,
        IUserLoginStore<User>,
        IUserSecurityStampStore<User>,
        IUserClaimStore<User>,
        IRoleStore<Role>,
        IUserPhoneNumberStore<User>,
        IUserTwoFactorStore<User>,
        IUserAuthenticatorKeyStore<User>
    {
        private readonly ISecurity _platformSecurityApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IMemberService _memberService;
        private readonly StorefrontOptions _options;
        private readonly IOrderModule _orderModule;

        public UserStoreStub(ISecurity platformSecurityApi,
            IMemberService memberService,
            IStorefrontMemoryCache memoryCache,
            IOptions<StorefrontOptions> options,
            IOrderModule orderModule)
        {
            _platformSecurityApi = platformSecurityApi;
            _memoryCache = memoryCache;
            _memberService = memberService;
            _options = options.Value;
            _orderModule = orderModule;
        }

        #region IUserStore<User> members
        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            if (user.Contact != null)
            {
                user.Contact = await _memberService.CreateContactAsync(user.Contact);
            }
            var dtoUser = user.ToUserDto();
            var resultDto = await _platformSecurityApi.CreateAsyncAsync(dtoUser);
            return resultDto.ToIdentityResult();
        }
        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            var result = IdentityResult.Success;
            await _platformSecurityApi.UpdateRoleAsync(role.ToRoleDto());
            return result;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            await _platformSecurityApi.DeleteAsyncAsync(new[] { user.UserName });
            //Evict user from the cache
            SecurityCacheRegion.ExpireUser(user.Id);
            return IdentityResult.Success;
        }
        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            var result = IdentityResult.Success;
            await _platformSecurityApi.UpdateRoleAsync(role.ToRoleDto());
            return result;
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByIdAsync", userId);
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _platformSecurityApi.GetUserByIdAsync(userId);

                return await PrepareUserResultAsync(cacheEntry, userDto);
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
            var result = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var userDto = await _platformSecurityApi.GetUserByNameAsync(normalizedUserName);

                return await PrepareUserResultAsync(cacheEntry, userDto);
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

                return await PrepareUserResultAsync(cacheEntry, userDto);
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

                return await PrepareUserResultAsync(cacheEntry, userDto);
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
            if (existUserLogin != null)
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

        #region IUserPhoneNumberStore<User> members
        public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        #endregion

        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        #region IUserClaimStore<User> members
        public Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            IList<Claim> result = new List<Claim>();
            if (user.IsAdministrator)
            {
                result.Add(new Claim(ClaimTypes.Role, SecurityConstants.Roles.Administrator));
            }

            if (user.SelectedCurrencyCode != null)
            {
                result.Add(new Claim(SecurityConstants.Claims.CurrencyClaimType, user.SelectedCurrencyCode));
            }

            if (!string.IsNullOrEmpty(user.OperatorUserName))
            {
                result.Add(new Claim(SecurityConstants.Claims.OperatorUserNameClaimType, user.OperatorUserName));
            }

            if (!string.IsNullOrEmpty(user.OperatorUserId))
            {
                result.Add(new Claim(SecurityConstants.Claims.OperatorUserIdClaimType, user.OperatorUserId));
                result.Add(new Claim(SecurityConstants.Claims.OperatorUserNameClaimType, user.OperatorUserName));
            }

            if (!user.Permissions.IsNullOrEmpty())
            {
                foreach (var permission in user.Permissions)
                {
                    result.Add(new Claim(SecurityConstants.Claims.PermissionClaimType, permission));
                }
            }
            if (!user.Roles.IsNullOrEmpty())
            {
                foreach (var role in user.Roles)
                {
                    result.Add(new Claim(ClaimTypes.Role, role.Id));
                }
            }
            return Task.FromResult(result);
        }

        public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            IList<User> result = new List<User>();
            return Task.FromResult(result);
        }
        #endregion

        #region IRoleStore<Role> members

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            role.Name = normalizedName;
            return Task.CompletedTask;
        }

        async Task<Role> IRoleStore<Role>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var result = (await _platformSecurityApi.GetRoleAsync(roleId))?.ToRole();
            return result;
        }

        Task<Role> IRoleStore<Role>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IUserAuthenticatorKeyStore<User>
        public Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken)
        {
            user.TwoFactorAuthenticatorKey = key;
            return Task.CompletedTask;
        }

        public Task<string> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorAuthenticatorKey);
        }
        #endregion IUserAuthenticatorKeyStore<User>

        public void Dispose()
        {
            // Cleanup
        }

        private async Task<User> PrepareUserResultAsync(MemoryCacheEntryOptions options, AutoRestClients.PlatformModuleApi.Models.ApplicationUserExtended userDto)
        {
            if (userDto != null)
            {
                var user = userDto.ToUser();
                var orderSearchResult = await _orderModule.SearchAsync(new CustomerOrderSearchCriteria()
                {
                    CustomerId = user.Id,
                    Take = 0,
                    Skip = 0,
                });

                user.IsFirstTimeBuyer = orderSearchResult.TotalCount == 0;

                options.AddExpirationToken(new PollingApiUserChangeToken(_platformSecurityApi, _options.ChangesPollingInterval));
                options.AddExpirationToken(SecurityCacheRegion.CreateChangeToken(userDto.Id));

                return user;
            }
            return null;
        }
    }
}
