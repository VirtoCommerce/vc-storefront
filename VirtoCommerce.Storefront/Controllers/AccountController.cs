using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute("account")]
    public class AccountController : StorefrontControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthorizationService _authorizationService;

        public AccountController(
            IWorkContextAccessor workContextAccessor,
            IStorefrontUrlBuilder urlBuilder,
            SignInManager<User> signInManager,
            IAuthorizationService authorizationService)
            : base(workContextAccessor, urlBuilder)
        {
            _signInManager = signInManager;
            _authorizationService = authorizationService;
        }

        // GET: /account/impersonate/{userId}
        [HttpGet("impersonate/{userId}")]
        public async Task<IActionResult> ImpersonateUser(string userId)
        {
            if (User.Identity.Name == SecurityConstants.AnonymousUsername || User.Claims.Any(x => x.Type == SecurityConstants.Claims.OperatorUserNameClaimType))
            {
                return StoreFrontRedirect($"~/sign-in?redirect={System.Uri.EscapeDataString(Request.Path)}");
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, null, CanImpersonateAuthorizationRequirement.PolicyName);

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var impersonateUser = await _signInManager.UserManager.FindByIdAsync(userId);

            if (impersonateUser != null)
            {
                impersonateUser.OperatorUserId = WorkContext.CurrentUser.Id;
                impersonateUser.OperatorUserName = WorkContext.CurrentUser.UserName;
                impersonateUser.OperatorFullName = WorkContext.CurrentUser.Name;

                // sign out the current user
                await _signInManager.SignOutAsync();

                await _signInManager.SignInAsync(impersonateUser, isPersistent: false);
            }

            return StoreFrontRedirect("~/");
        }
    }
}
