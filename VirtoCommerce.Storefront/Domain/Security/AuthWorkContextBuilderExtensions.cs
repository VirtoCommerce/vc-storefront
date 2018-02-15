using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public static class AuthWorkContextBuilderExtensions
    {
        public static async Task WithCurrentUserAsync(this IWorkContextBuilder builder)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var signInManager = serviceProvider.GetRequiredService<SignInManager<User>>();

            // Gets the collection of external login providers
            var externalAuthTypes = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            builder.WorkContext.ExternalLoginProviders = externalAuthTypes.Select(at => new LoginProvider
            {
                AuthenticationType = at.Name,
                Caption = at.DisplayName,
            }).ToList();

            var user = new User
            {
                Id = builder.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = builder.HttpContext.User.FindFirstValue(ClaimTypes.Name)
            };

            var identity = builder.HttpContext.User.Identity;
            if (identity.IsAuthenticated)
            {
                user = await signInManager.UserManager.FindByNameAsync(identity.Name);
                //User has been removed from storage or current store is not allowed for signed in user
                //need to do sign out 
                if (user == null || !user.AllowedStores.Contains(builder.WorkContext.CurrentStore.Id))
                {
                    await signInManager.SignOutAsync();
                    user = null;
                }              
            }

            if (user == null || user.IsTransient())
            {
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = StorefrontClaims.AnonymousUsername,
                };
                //Workaround: Do not sign out for js map requests they are always coming without authentication
                if (!builder.HttpContext.Request.Path.Value.EndsWith(".map"))
                {
                    //Sign-in anonymous user
                    await signInManager.SignInAsync(user, false);
                }
            }
            //Restore some properties from claims
            user.OperatorUserId = builder.HttpContext.User.FindFirstValue(StorefrontClaims.OperatorUserIdClaimType);
            user.OperatorUserName = builder.HttpContext.User.FindFirstValue(StorefrontClaims.OperatorUserNameClaimType);
            user.SelectedCurrencyCode = builder.HttpContext.User.FindFirstValue(StorefrontClaims.CurrencyClaimType);

            builder.WorkContext.CurrentUser = user;
        }
    }
}
