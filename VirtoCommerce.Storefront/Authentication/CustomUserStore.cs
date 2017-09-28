using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using securityDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using VirtoCommerce.Storefront.Extensions;

namespace VirtoCommerce.Storefront.Authentication
{

    public class CustomUserStore :  IUserStore<CustomerInfo>,
                                    IUserEmailStore<CustomerInfo>,
                                    IUserPasswordStore<CustomerInfo>,
                                    IUserLoginStore<CustomerInfo>,
                                    IUserPhoneNumberStore<CustomerInfo>,
                                    IUserTwoFactorStore<CustomerInfo>
    {
        private readonly IStorefrontSecurity _commerceCoreApi;
        private readonly ICustomerService _customerService;
        private readonly IMemoryCache _memoryCache;

        public CustomUserStore(IStorefrontSecurity commerceCoreApi, ICustomerService customerService, IMemoryCache memoryCache)
        {
            _commerceCoreApi = commerceCoreApi;
            _customerService = customerService;
            _memoryCache = memoryCache;
        }

        public async Task<IdentityResult> CreateAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            var dtoUser = new securityDto.ApplicationUserExtended
            {
                Email = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                UserType = "Customer",
                StoreId = user.StoreId
            };
            var result = await _commerceCoreApi.CreateAsync(dtoUser);

            if (result.Succeeded == true)
            {
                //Load newly created account from API
                var storefrontUser = await _commerceCoreApi.GetUserByNameAsync(user.UserName);

                //Next need create corresponding Customer contact in VC Customers (CRM) module
                user.Id = storefrontUser.Id;
                user.IsRegisteredUser = true;
                user.AllowedStores = storefrontUser.AllowedStores;
                await _customerService.CreateCustomerAsync(user);
                return IdentityResult.Success;
            }
            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> UpdateAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            var fullName = string.Join(" ", user.FirstName, user.LastName).Trim();
            if (string.IsNullOrEmpty(fullName))
            {
                fullName = user.Email;
            }
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                user.FullName = fullName;
            }
            var addresses = user.Addresses;
            user.Addresses = null;
            await _customerService.UpdateCustomerAsync(user);
            user.Addresses = addresses;
            await _customerService.UpdateAddressesAsync(user);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomerInfo> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByIdAsync", userId);
            var user = await _memoryCache.GetOrCreateAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(UserStoreCacheRegion.CreateChangeToken());
                return await _commerceCoreApi.GetUserByIdAsync(userId, cancellationToken);
            }, cacheNullValue: false);

            if (user != null)
            {
                return await GetContactInfoForPlatformUserAsync(user);
            }
            return null;
        }

        public async Task<CustomerInfo> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByNameAsync", normalizedUserName);
            var user = await _memoryCache.GetOrCreateAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(UserStoreCacheRegion.CreateChangeToken());
                return await _commerceCoreApi.GetUserByNameAsync(normalizedUserName, cancellationToken);
            }, cacheNullValue: false);

            if (user != null)
            {
                return await GetContactInfoForPlatformUserAsync(user);
            }
            return null;
        }

        private async Task<CustomerInfo> GetContactInfoForPlatformUserAsync(securityDto.StorefrontUser user)
        {
            CustomerInfo result = null;
            if (!string.IsNullOrEmpty(user.MemberId))
            {
                result = await _customerService.GetCustomerByIdAsync(user.MemberId);
            }

            // User may not have contact record
            if (result == null)
            {
                result = new CustomerInfo
                {
                    Id = user.Id,
                };
            }
            result.MemberId = user.MemberId;
            result.UserName = user.UserName;
            result.AllowedStores = user.AllowedStores;
            result.IsRegisteredUser = true;
            return result;
        }

        public Task<string> GetUserIdAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetNormalizedUserNameAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserNameNormalized);
        }

        public Task SetEmailAsync(CustomerInfo user, string email, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<string> GetEmailAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(CustomerInfo user, bool confirmed, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<CustomerInfo> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(CustomerInfo user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<string> GetPasswordHashAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(CustomerInfo user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;

            return Task.FromResult(true);
        }

        public Task SetNormalizedUserNameAsync(CustomerInfo user, string normalizedName, CancellationToken cancellationToken)
        {
            // Do nothing. In this simple example, the normalized user name is generated from the user name.
            return Task.FromResult(true);
        }

        public Task SetPasswordHashAsync(CustomerInfo user, string passwordHash, CancellationToken cancellationToken)
        {
            //Nothing todo
            return Task.FromResult(true);
        }

        public Task<string> GetPhoneNumberAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(CustomerInfo user, string phoneNumber, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(CustomerInfo user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(CustomerInfo user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;

            return Task.FromResult(true);
        }

        public Task<bool> GetTwoFactorEnabledAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            // Just returning an empty list because I don't feel like implementing this. You should get the idea though...
            IList<UserLoginInfo> logins = new List<UserLoginInfo>();
            return Task.FromResult(logins);
        }

        public async Task<CustomerInfo> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey.With(GetType(), "FindByLoginAsync", loginProvider, providerKey);
            var user = await _memoryCache.GetOrCreateAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(UserStoreCacheRegion.CreateChangeToken());
                return await _commerceCoreApi.GetUserByLoginAsync(loginProvider, providerKey);
            });

            if (user != null)
            {
                return await GetContactInfoForPlatformUserAsync(user);
            }
            return null;
        }

        public async Task AddLoginAsync(CustomerInfo user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            var newUser = new securityDto.ApplicationUserExtended
            {
                Email = user.Email,
                UserName = user.UserName,
                UserType = "Customer",
                StoreId = user.StoreId,
                Logins = new List<securityDto.ApplicationUserLogin>
                    {
                        new securityDto.ApplicationUserLogin
                        {
                            LoginProvider = login.LoginProvider,
                            ProviderKey = login.ProviderKey
                        }
                    }
            };
            var result = await _commerceCoreApi.CreateAsync(newUser);

            if (result.Succeeded == true)
            {
                var storefrontUser = await _commerceCoreApi.GetUserByNameAsync(user.UserName);

                user.Id = storefrontUser.Id;
                user.IsRegisteredUser = true;
                user.MemberType = "Contact";
                user.FullName = user.FullName;
                user.AllowedStores = storefrontUser.AllowedStores;
                await _customerService.CreateCustomerAsync(user);
            }

            var cacheKey = CacheKey.With(GetType(), "FindByLoginAsync", login.LoginProvider, login.ProviderKey);
            _memoryCache.Remove(cacheKey);
        }

        public Task RemoveLoginAsync(CustomerInfo user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }

    }
}
