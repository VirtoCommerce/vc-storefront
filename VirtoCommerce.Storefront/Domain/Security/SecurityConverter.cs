using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using securityDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using platformSecurityDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Common;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public static class SecurityConverterExtensions
    {
        public static SecurityConverter ConverterInstance =>
            new SecurityConverter();

        public static securityDto.ApplicationUserExtended ToUserDto(this User user, IEnumerable<platformSecurityDto.Role> allPlatformRoles = null)
        {
            return ConverterInstance.ToUserDto(user, allPlatformRoles);
        }
        public static platformSecurityDto.ApplicationUserExtended ToPlatformUserDto(this User user, IEnumerable<platformSecurityDto.Role> allPlatformRoles = null)
        {
            return ConverterInstance.ToUserDto(user, allPlatformRoles).JsonConvert<platformSecurityDto.ApplicationUserExtended>();
        }

        public static User ToUser(this securityDto.StorefrontUser userDto)
        {
            return ConverterInstance.ToUser(userDto);
        }

        public static User ToUser(this Register registerForm)
        {
            return ConverterInstance.ToUser(registerForm);
        }

        public static UserRegistrationInfo ToUserRegistrationInfo(this Register registerForm)
        {
            return ConverterInstance.ToUserRegistrationInfo(registerForm);
        }
        public static IdentityResult ToIdentityResult(this securityDto.SecurityResult resultDto)
        {
            return ConverterInstance.ToIdentityResult(resultDto);
        }
        public static IdentityResult ToIdentityResult(this platformSecurityDto.SecurityResult resultDto)
        {
            return ConverterInstance.ToIdentityResult(resultDto.JsonConvert<securityDto.SecurityResult>());
        }
    }

    public class SecurityConverter
    {
        public virtual IdentityResult ToIdentityResult(securityDto.SecurityResult resultDto)
        {
            if (resultDto.Succeeded == true)
            {
                return IdentityResult.Success;
            }
            return IdentityResult.Failed(resultDto.Errors.Select(x => new IdentityError { Description = x }).ToArray());
        }

        public virtual UserRegistrationInfo ToUserRegistrationInfo(Register registerForm)
        {
            var result = new UserRegistrationInfo
            {
                Email = registerForm.Email,
                FirstName = registerForm.FirstName,
                LastName = registerForm.LastName,
                Password = registerForm.Password,
                UserName = registerForm.UserName,
                Address = registerForm.Address,
                Name = registerForm.Name,
                ExistOrganizationId = registerForm.OrganizationId,
                NewOrganizationName = registerForm.OrganizationName,
                Role = registerForm.Role,
                StoreId = registerForm.StoreId
            };
            return result;

        }
        public virtual User ToUser(Register registerForm)
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

        public virtual securityDto.ApplicationUserExtended ToUserDto(User user, IEnumerable<platformSecurityDto.Role> allPlatformRoles = null)
        {
            var result = new securityDto.ApplicationUserExtended
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
                TwoFactorEnabled = user.TwoFactorEnabled
            };

            if (!user.Roles.IsNullOrEmpty() && allPlatformRoles != null)
            {
                //Need to convert role names to the registered in the platform roles entities 
                result.Roles = allPlatformRoles.Join(user.Roles, x => x.Name, y => y, (x, y) => new securityDto.Role { Id = x.Id }).ToList();
            }
            if (!user.ExternalLogins.IsNullOrEmpty())
            {
                result.Logins = user.ExternalLogins.Select(x => new securityDto.ApplicationUserLogin
                {
                    LoginProvider = x.LoginProvider,
                    ProviderKey = x.ProviderKey
                }).ToList();
            }
            return result;
        }

        public virtual User ToUser(securityDto.StorefrontUser userDto)
        {
            var result = new User()
            {
                AllowedStores = userDto.AllowedStores,
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
                LockoutEndDateUtc = userDto.LockoutEndDateUtc
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
