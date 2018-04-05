using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using dto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain.Security
{

    public static class SecurityConverter
    {
        public static IdentityResult ToIdentityResult(this dto.SecurityResult resultDto)
        {
            if (resultDto.Succeeded == true)
            {
                return IdentityResult.Success;
            }
            return IdentityResult.Failed(resultDto.Errors.Select(x => new IdentityError { Description = x }).ToArray());
        }

       
        public static  User ToUser(this UserRegistration registerForm)
        {
            var result = new User
            {
                Email = registerForm.Email,
                UserName = registerForm.UserName,
                Password = registerForm.Password,
            };
            if (!string.IsNullOrEmpty(registerForm.Role))
            {
                result.Roles = new[] { registerForm.Role };
            }

            return result;
        }

        public static dto.ApplicationUserExtended ToUserDto(this User user, IEnumerable<dto.Role> allPlatformRoles = null)
        {
            var result = new dto.ApplicationUserExtended
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                UserType = "Customer",
                StoreId = user.StoreId,
                MemberId = user.ContactId,
                AccessFailedCount = user.AccessFailedCount,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEndDateUtc,
                TwoFactorEnabled = user.TwoFactorEnabled,
                SecurityStamp = user.SecurityStamp,
                PasswordHash = user.PasswordHash
            };

            if (!user.Roles.IsNullOrEmpty() && allPlatformRoles != null)
            {
                //Need to convert role names to the registered in the platform roles entities 
                result.Roles = allPlatformRoles.Join(user.Roles, x => x.Name, y => y, (x, y) => new dto.Role { Id = x.Id }).ToList();
            }
            if (!user.ExternalLogins.IsNullOrEmpty())
            {
                result.Logins = user.ExternalLogins.Select(x => new dto.ApplicationUserLogin
                {
                    LoginProvider = x.LoginProvider,
                    ProviderKey = x.ProviderKey
                }).ToList();
            }
            return result;
        }

        public static User ToUser(this dto.ApplicationUserExtended userDto)
        {
            var result = new User()
            {
                Email = userDto.Email,
                Id = userDto.Id,
                ContactId = userDto.MemberId,
                PhoneNumber = userDto.PhoneNumber,
                UserName = userDto.UserName,
                StoreId = userDto.StoreId,
                IsRegisteredUser = true,
                IsAdministrator = userDto.IsAdministrator ?? false,
                Permissions = userDto.Permissions,
                Roles = userDto.Roles?.Select(x => x.Name),
                AccessFailedCount = userDto.AccessFailedCount ?? 0,
                LockoutEnabled = userDto.LockoutEnabled ?? false,
                EmailConfirmed = userDto.EmailConfirmed ?? false,
                LockoutEndDateUtc = userDto.LockoutEndDateUtc,
                PasswordHash = userDto.PasswordHash,
                SecurityStamp = userDto.SecurityStamp,
            };

            if (!userDto.Logins.IsNullOrEmpty())
            {
                result.ExternalLogins = userDto.Logins.Select(x => new ExternalUserLoginInfo
                {
                    LoginProvider = x.LoginProvider,
                    ProviderKey = x.ProviderKey
                }).ToList();
            }

            return result;
        }
    }

}
