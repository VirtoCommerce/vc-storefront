using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using securityDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using platformSecurityDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Common;
using Microsoft.AspNetCore.Identity;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public static class SecurityConverterExtensions
    {
        public static SecurityConverter ConverterInstance =>
            new SecurityConverter();

        public static securityDto.ApplicationUserExtended ToUserDto(this User user)
        {
            return ConverterInstance.ToUserDto(user);
        }
        public static platformSecurityDto.ApplicationUserExtended ToPlatformUserDto(this User user)
        {
            return ConverterInstance.ToUserDto(user).JsonConvert<platformSecurityDto.ApplicationUserExtended>();
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
                UserName = registerForm.UserName
            };
            return result;

        }
        public virtual User ToUser(Register registerForm)
        {
            var result = new User
            {
                Email = registerForm.Email,
                UserName = registerForm.UserName,
                Password = registerForm.Password
            };
         

            return result;
        }

        public virtual securityDto.ApplicationUserExtended ToUserDto(User user)
        {
            var result = new securityDto.ApplicationUserExtended
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                UserType = "Customer",
                StoreId = user.StoreId,
                MemberId = user.ContactId
            };
            if(!user.ExternalLogins.IsNullOrEmpty())
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
                IsRegisteredUser = true
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
