using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CustomUserManager : AspNetUserManager<User>
    {
        private readonly IStorefrontSecurityService _storefrontSecurity;
        private readonly IMemoryCache _memoryCache;
        public CustomUserManager(IStorefrontSecurityService storefrontSecurity, IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher,
                                IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators,
                                ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _storefrontSecurity = storefrontSecurity;
        }

        public override async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _storefrontSecurity.CheckPasswordAsync(user, password);
        }
    
        public override async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            var result = await _storefrontSecurity.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded == true)
            {
                return IdentityResult.Success;
            }
            return IdentityResult.Failed(result.Errors.Select(x=> new IdentityError { Description = x }).ToArray());
        }

        public override async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            var result = await _storefrontSecurity.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded == true)
            {
                return IdentityResult.Success;
            }
            return IdentityResult.Failed(result.Errors.Select(x => new IdentityError { Description = x }).ToArray());
        }

    }
}
