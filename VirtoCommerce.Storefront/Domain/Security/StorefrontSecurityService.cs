using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using platformSecurityDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using System.Collections.Generic;
using System;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class StorefrontSecurityService : IStorefrontSecurityService
    {
        private readonly IStoreModule _storeApi;
        private readonly IStorefrontSecurity _commerceCoreApi;
        private readonly ISecurity _platformSecurityApi;
        public StorefrontSecurityService(IStoreModule storeApi, IStorefrontSecurity commerceCoreApi, ISecurity platformSecurityApi)
        {
            _storeApi = storeApi;
            _commerceCoreApi = commerceCoreApi;
            _platformSecurityApi = platformSecurityApi;
        }
        public virtual async Task<bool> CanLoginOnBehalfAsync(string storeId, string userId)
        {
            var info = await _storeApi.GetLoginOnBehalfInfoAsync(storeId, userId);
            return info.CanLoginOnBehalf == true;
        }

        public async Task<SecurityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            var changePassword = new platformSecurityDto.ChangePasswordInfo
            {
                OldPassword = currentPassword,
                NewPassword = newPassword,
            };

            var resultDto = await _platformSecurityApi.ChangePasswordAsync(user.UserName, changePassword);

            var result = new SecurityResult
            {
                Succeeded = resultDto.Succeeded ?? false,
                Errors = resultDto.Errors ?? new List<string>()
            };
            return result;
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var result = await _commerceCoreApi.PasswordSignInAsync(user.UserName, password);

            return result.Status.EqualsInvariant("success");
        }

        public async Task<SecurityResult> CreateAsync(User user)
        {
            var dtoUser = user.ToUserDto();
            var resultDto = await _commerceCoreApi.CreateAsync(dtoUser);
            var result = new SecurityResult
            {
                Succeeded = resultDto.Succeeded ?? false,
                Errors = resultDto.Errors
            };
            return result;
        }

        public async Task GeneratePasswordResetTokenAsync(string userId, string storeId, Language language, string callbackUrl)
        {
            if(userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (storeId == null)
            {
                throw new ArgumentNullException(nameof(storeId));
            }
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }
            if (callbackUrl == null)
            {
                throw new ArgumentNullException(nameof(callbackUrl));
            }

            await _commerceCoreApi.GenerateResetPasswordTokenAsync(userId, storeId, language.CultureName, callbackUrl);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            User result = null;
            var userDto = await _commerceCoreApi.GetUserByEmailAsync(email);
            if (userDto != null)
            {
                result = userDto.ToUser();
            }
            return result;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            User result = null;
            var userDto = await _commerceCoreApi.GetUserByIdAsync(userId);
            if(userDto != null)
            {
                result = userDto.ToUser();
            }
            return result;
        }

        public async Task<User> GetUserByLoginAsync(string loginProvider, string providerKey)
        {
            User result = null;
            var userDto = await _commerceCoreApi.GetUserByLoginAsync(loginProvider, providerKey);
            if (userDto != null)
            {
                result = userDto.ToUser();
            }
            return result;
        }

        public async Task<User> GetUserByNameAsync(string userName)
        {
            User result = null;
            var userDto = await _commerceCoreApi.GetUserByNameAsync(userName);
            if (userDto != null)
            {
                result = userDto.ToUser();
            }
            return result;
        }

        public async Task<bool> PasswordSignInAsync(string userName, string password)
        {
            if(userName == null)
            {
                throw new ArgumentNullException(nameof(userName));
            }

            var result = await _commerceCoreApi.PasswordSignInAsync(userName, password);

            return result.Status?.EqualsInvariant("success") ?? false;            

        }

        public async Task<SecurityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            if (newPassword == null)
            {
                throw new ArgumentNullException(nameof(newPassword));
            }
            var resultDto = await _commerceCoreApi.ResetPasswordAsync(user.Id, token, newPassword);
            var result = new SecurityResult
            {
                Succeeded = resultDto.Succeeded ?? false,
                Errors = resultDto.Errors
            };
            return result;
        }
    }
}
