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

        public static dto.Role ToRoleDto(this Role role)
        {
            return new dto.Role
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = role?.Permissions.Select(x => new dto.Permission { Id = x, Name = x }).ToList()
            };
        }
        public static Role ToRole(this dto.Role roleDto)
        {
            return new Role
            {
                Id = roleDto.Id,
                Name = roleDto.Name,
                Permissions = roleDto?.Permissions.Select(x => x.Id).ToList()
            };
        }

        public static User ToUser(this OrganizationUserRegistration registerForm)
        {
            var result = ((UserRegistration)registerForm).ToUser();
            if (!string.IsNullOrEmpty(registerForm.Role))
            {
                var role = SecurityConstants.Roles.AllRoles.FirstOrDefault(x => x.Id.EqualsInvariant(registerForm.Role));
                if (role != null)
                {
                    result.Roles = new[] { role };
                }
                else
                {
                    result.Roles = new[] { new Role { Id = registerForm.Role, Name = registerForm.Role } };
                }
            }
            return result;
        }

        public static User ToUser(this UserRegistration registerForm)
        {
            var result = new User
            {
                Email = registerForm.Email,
                UserName = registerForm.UserName,
                Password = registerForm.Password,
                UserType = "Customer"
            };
            //Take userName as Email if it valid and Email is not set 
            if (string.IsNullOrEmpty(result.Email))
            {
                result.Email = registerForm.UserName.IsValidEmail() ? registerForm.UserName : result.Email;
            }
            return result;
        }

        public static dto.ApplicationUser ToUserDto(this User user)
        {
            var result = new dto.ApplicationUser
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                StoreId = user.StoreId,
                MemberId = user.Contact?.Id ?? user.ContactId,
                AccessFailedCount = user.AccessFailedCount,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEndDateUtc,
                TwoFactorEnabled = user.TwoFactorEnabled,
                SecurityStamp = user.SecurityStamp,
                PasswordHash = user.PasswordHash,
                UserState = user.UserState.ToString(),
                UserType = user.UserType,
                IsAdministrator = user.IsAdministrator,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            };

            if (!user.Roles.IsNullOrEmpty())
            {
                //Need to convert role names to the registered in the platform roles entities 
                result.Roles = user.Roles.Select(ToRoleDto).ToList();
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

        public static User ToUser(this dto.ApplicationUser userDto)
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
                AccessFailedCount = userDto.AccessFailedCount ?? 0,
                LockoutEnabled = userDto.LockoutEnabled ?? false,
                EmailConfirmed = userDto.EmailConfirmed ?? false,
                LockoutEndDateUtc = userDto.LockoutEndDateUtc,
                PasswordExpired = userDto.PasswordExpired ?? false,
                PasswordHash = userDto.PasswordHash,
                SecurityStamp = userDto.SecurityStamp,
                UserState = EnumUtility.SafeParse(userDto.UserState, AccountState.Approved),
                UserType = userDto.UserType,
                TwoFactorEnabled = userDto.TwoFactorEnabled ?? false,
                PhoneNumberConfirmed = userDto.PhoneNumberConfirmed ?? false,
            };

            if (!userDto.Roles.IsNullOrEmpty())
            {
                result.Roles = userDto.Roles.Select(x => new Role
                {
                    Id = x.Id,
                    Name = x.Name
                });
            }

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
