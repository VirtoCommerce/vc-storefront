using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Security.Specifications;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public static class SecurityWorkContextBuilderExtensions
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

            var user = await signInManager.UserManager.GetUserAsync(builder.HttpContext.User);
            //User doesn't have permissions for login to current store 
            //need to do sign out 
            if (user != null && !new CanUserLoginToStoreSpecification(user).IsSatisfiedBy(builder.WorkContext.CurrentStore))
            {
                await signInManager.SignOutAsync();
                user = null;
            }
            //Login as a new anonymous user
            if (user == null || user.IsTransient())
            {
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = SecurityConstants.AnonymousUsername,
                };
                //Workaround: Do not sign out for js map requests they are always coming without authentication
                if (!builder.HttpContext.Request.Path.Value.EndsWith(".map"))
                {
                    //Sign-in anonymous user
                    await signInManager.SignInAsync(user, false);
                }
            }
            builder.WorkContext.CurrentUser = user;
        }


    }
}
