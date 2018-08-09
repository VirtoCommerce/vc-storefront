using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
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

        public async override Task<User> GetUserAsync(ClaimsPrincipal principal)
        {
            //User can be anonymous and also should be signed-in 
            var user = new User
            {
                Id = principal.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = principal.FindFirstValue(ClaimTypes.Name),
                SecurityStamp = principal.FindFirstValue("AspNet.Identity.SecurityStamp")
            };
            //For registered users need to load it  from storage
            if (principal.Identity.IsAuthenticated && user.UserName != SecurityConstants.AnonymousUsername)
            {
                user = await FindByIdAsync(user.Id);
            }
            //Restore some properties from cookies
            if (user != null)
            {
                user.OperatorUserId = principal.FindFirstValue(SecurityConstants.Claims.OperatorUserIdClaimType);
                user.OperatorUserName = principal.FindFirstValue(SecurityConstants.Claims.OperatorUserNameClaimType);
                user.SelectedCurrencyCode = principal.FindFirstValue(SecurityConstants.Claims.CurrencyClaimType);
            }
            return user;
        }
    }
}
