using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Customer;

namespace VirtoCommerce.Storefront.Authentication
{
    public static class AuthWorkContextBuilderExtensions
    {
        public static async Task WithCurrentUserAsync(this IWorkContextBuilder builder)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var signInManager = serviceProvider.GetRequiredService<SignInManager<CustomerInfo>>();

            var customer = new CustomerInfo
            {
                Id = builder.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = builder.HttpContext.User.FindFirstValue(ClaimTypes.Name)
            };

            var identity = builder.HttpContext.User.Identity;
            if (identity.IsAuthenticated)
            {
                customer = await signInManager.UserManager.FindByNameAsync(identity.Name);
                //User has been removed from storage need to do sign out 
                if (customer == null)
                {
                    await signInManager.SignOutAsync();
                }
            }

            if (customer == null || customer.IsTransient())
            {
                customer = new CustomerInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = StorefrontClaims.AnonymousUsername,
                    FullName = StorefrontClaims.AnonymousUsername
                };
                //Sign-in anonymous user
                await signInManager.SignInAsync(customer, true);
            }
            //Restore some properties from claims
            customer.OperatorUserId = builder.HttpContext.User.FindFirstValue(StorefrontClaims.OperatorUserIdClaimType);
            customer.OperatorUserName = builder.HttpContext.User.FindFirstValue(StorefrontClaims.OperatorUserNameClaimType);
            customer.SelectedCurrencyCode = builder.HttpContext.User.FindFirstValue(StorefrontClaims.CurrencyClaimType);

            builder.WorkContext.CurrentCustomer = customer;
        }
    }
}
