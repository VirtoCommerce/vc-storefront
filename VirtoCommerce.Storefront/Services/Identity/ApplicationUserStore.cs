using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using securityDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Services.Identity
{

    public class ApplicationUserStore : IUserStore<CustomerInfo>,
                                    IUserEmailStore<CustomerInfo>,
                                    IUserPasswordStore<CustomerInfo>,
                                    IUserLoginStore<CustomerInfo>,
                                    IUserPhoneNumberStore<CustomerInfo>,
                                    IUserTwoFactorStore<CustomerInfo>
    {
        private readonly IStorefrontSecurity _commerceCoreApi;
        private readonly ICustomerService _customerService;
        public ApplicationUserStore(IStorefrontSecurity commerceCoreApi, ICustomerService customerService)
        {
            _commerceCoreApi = commerceCoreApi;
            _customerService = customerService;
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
                user.IsRegisteredUser = true;
                user.AllowedStores = storefrontUser.AllowedStores;
                await _customerService.CreateCustomerAsync(user);
                return IdentityResult.Success;
            }
            return IdentityResult.Failed();
        }

        public Task<IdentityResult> UpdateAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomerInfo> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _commerceCoreApi.GetUserByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                return await GetCustomerInfoForPlatformUserAsync(user);
            }            
            return null;
        }

        public async Task<CustomerInfo> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await _commerceCoreApi.GetUserByNameAsync(normalizedUserName, cancellationToken);
            if (user != null)
            {
                return await GetCustomerInfoForPlatformUserAsync(user);
            }
            return null;
        }

        private async Task<CustomerInfo> GetCustomerInfoForPlatformUserAsync(securityDto.StorefrontUser user)
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
                    IsRegisteredUser = true,
                };
            }
            result.UserName = user.UserName;
            result.AllowedStores = user.AllowedStores;
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
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(CustomerInfo user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
           
            return Task.FromResult(true);
        }

        public Task<string> GetPhoneNumberAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task SetPhoneNumberAsync(CustomerInfo user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;

            return Task.FromResult(true);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(CustomerInfo user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(CustomerInfo user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(true);
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

        public Task<CustomerInfo> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddLoginAsync(CustomerInfo user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(CustomerInfo user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose() { }

    }
}
