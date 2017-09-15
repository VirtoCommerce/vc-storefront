using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Customer;

namespace VirtoCommerce.Storefront.Filters
{
    public class WorkContextAuthPopulateFilter : IAsyncActionFilter
    {
        private IWorkContextAccessor _workContextAccessor;
        public WorkContextAuthPopulateFilter(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cutomerInfo = new CustomerInfo();
            var workContext = _workContextAccessor.WorkContext;

            var identity = context.HttpContext.User.Identity;

            if (identity.IsAuthenticated)
            {
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<CustomerInfo>>();

                workContext.CurrentCustomer = await userManager.FindByNameAsync(identity.Name);
                var claimsPrincipal = identity as ClaimsPrincipal;
                if (claimsPrincipal != null)
                {
                    cutomerInfo.OperatorUserId = claimsPrincipal.FindFirstValue(StorefrontConstants.OperatorUserIdClaimType);
                    cutomerInfo.OperatorUserName = claimsPrincipal.FindFirstValue(StorefrontConstants.OperatorUserNameClaimType);
                }
            }
            if (!cutomerInfo.IsRegisteredUser)
            {
                cutomerInfo.Id = identity.Name;
                cutomerInfo.UserName = StorefrontConstants.AnonymousUsername;
                cutomerInfo.FullName = StorefrontConstants.AnonymousUsername;
            }
            workContext.CurrentCustomer = cutomerInfo;

            await next();
        }
    }
}
