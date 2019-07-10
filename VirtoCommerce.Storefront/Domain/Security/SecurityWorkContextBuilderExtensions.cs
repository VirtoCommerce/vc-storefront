using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Security;

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
                    await signInManager.SignInAsync(user, isPersistent: true);
                    //https://github.com/aspnet/Security/issues/1131
                    //the sign in operation doesn't change the current request user principal.
                    //That only happens on incoming requests once the cookie or bearer token (or whatever thing the type of auth requires to create an identity) is set.
                    //Need to manually set User in the HttpContext to avoid issues such like Antiforegery token generation for undefined user for  the current request 
                    builder.HttpContext.User = await signInManager.ClaimsFactory.CreateAsync(user);
                }
            }
            builder.WorkContext.CurrentUser = user;
        }


    }
}
