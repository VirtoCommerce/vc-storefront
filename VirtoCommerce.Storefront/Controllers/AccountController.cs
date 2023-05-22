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
        /// <summary>
        /// Handles the impersonation functionality. It allows an authenticated user to impersonate another user by specifying the userId. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("impersonate/{userId}")]
        public async Task<IActionResult> ImpersonateUser(string userId)
        {
            // If the user is not authenticated, redirect to the sign-in page
            if (User.Identity.Name == SecurityConstants.AnonymousUsername)
            {
                return StoreFrontRedirect($"~/sign-in?returnUrl={System.Uri.EscapeDataString(Request.Path)}");
            }

            // If the user is not impersonating any user, check permission
            if (string.IsNullOrEmpty(WorkContext.CurrentUser.OperatorUserId))
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, null, CanImpersonateAuthorizationRequirement.PolicyName);

                if (!authorizationResult.Succeeded)
                {
                    return Forbid();
                }
            }

            // find the user to impersonate
            var impersonateUser = await _signInManager.UserManager.FindByIdAsync(userId);

            // if the user is found, update the impersonated user and sign in
            if (impersonateUser == null)
            {
                return NotFound();
            }

            UpdateImpersonatedUser(impersonateUser);
            await SignOutAndSignInAsync(impersonateUser);

            // redirect to the home page
            return StoreFrontRedirect("~/");
        }

        // GET: /account/impersonate/reset
        /// <summary>
        /// Resets the impersonation state and restore the original user's identity after being impersonated.
        /// </summary>
        /// <returns></returns>
        [HttpGet("impersonate/reset")]
        public async Task<IActionResult> ResetImpersonatation()
        {
            var operatorUserId = WorkContext.CurrentUser.OperatorUserId;

            if (string.IsNullOrEmpty(operatorUserId))
            {
                return StoreFrontRedirect("~/");
            }

            var operatorUser = await _signInManager.UserManager.FindByIdAsync(operatorUserId);

            if (operatorUser == null)
            {
                return NotFound();
            }

            await SignOutAndSignInAsync(operatorUser);

            return StoreFrontRedirect("~/");
        }

        /// <summary>
        /// Update operator to the impersonated user or keep current the operator if already impersonating
        /// </summary>
        /// <param name="impersonateUser"></param>
        private void UpdateImpersonatedUser(User impersonateUser)
        {
            impersonateUser.OperatorUserId = WorkContext.CurrentUser.OperatorUserId ?? WorkContext.CurrentUser.Id;
            impersonateUser.OperatorUserName = WorkContext.CurrentUser.OperatorUserName ?? WorkContext.CurrentUser.UserName;
            impersonateUser.OperatorFullName = WorkContext.CurrentUser.OperatorFullName ?? WorkContext.CurrentUser.Name;
        }

        /// <summary>
        /// Sign out the current user and sign in the impersonated user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task SignOutAndSignInAsync(User user)
        {
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, isPersistent: false);
        }
    }
}
