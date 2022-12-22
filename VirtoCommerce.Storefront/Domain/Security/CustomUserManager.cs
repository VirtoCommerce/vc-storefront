using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CustomUserManager : AspNetUserManager<User>
    {
        private readonly ISecurity _platformSecurityApi;

        public CustomUserManager(IUserStore<User> userStore, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher,
                                IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators,
                                ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,
                                ISecurity platformSecurityApi)
            : base(userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _platformSecurityApi = platformSecurityApi;
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

        public override async Task<Microsoft.AspNetCore.Identity.IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            var result = await _platformSecurityApi.ResetPasswordByTokenAsync(user.Id,
                new ResetPasswordConfirmRequest() { NewPassword = newPassword, Token = token });

            return result.ToIdentityResult();
        }

        public override async Task<Microsoft.AspNetCore.Identity.IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            var result = await _platformSecurityApi.ConfirmEmailAsync(user.Id, new ConfirmEmailRequest() { Token = token });

            return result.ToIdentityResult();
        }

        public override async Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail)
        {
            return await _platformSecurityApi.GenerateChangeEmailTokenAsync(user.Id, newEmail);
        }

        public override async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _platformSecurityApi.GenerateEmailConfirmationTokenAsync(user.Id);
        }

        public override async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _platformSecurityApi.GeneratePasswordResetTokenAsync(user.Id);
        }

        public override async Task<string> GenerateUserTokenAsync(User user, string tokenProvider, string purpose)
        {
            return await _platformSecurityApi.GenerateUserTokenAsync(user.Id, tokenProvider, purpose);
        }

        public override async Task<bool> VerifyUserTokenAsync(User user, string tokenProvider, string purpose, string token)
        {
            return await _platformSecurityApi.VerifyUserTokenAsync(user.Id,
                new VerifyTokenRequest() { TokenProvider = tokenProvider, Purpose = purpose, Token = tokenProvider }) ?? false;
        }
    }
}
