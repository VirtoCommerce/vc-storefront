using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Security.Events;

namespace VirtoCommerce.Storefront.Controllers
{
    [Authorize]
    public class AccountController : StorefrontControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IEventPublisher _publisher;
        private readonly IStorefrontSecurity _commerceCoreApi;
        public AccountController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, SignInManager<User> signInManager, IEventPublisher publisher, IStorefrontSecurity commerceCoreApi)
            : base(workContextAccessor, urlBuilder)
        {
            _signInManager = signInManager;
            _publisher = publisher;
            _commerceCoreApi = commerceCoreApi;
        }

        //GET: /account
        [HttpGet]
        public ActionResult GetAccount()
        {
            //Customer should be already populated in WorkContext middle-ware
            return View("customers/account", WorkContext);
        }


        [HttpGet]
        public ActionResult GetOrderDetails(string number)
        {
            var order = WorkContext.CurrentUser?.Contact?.Value?.Orders.FirstOrDefault(x => x.Number.EqualsInvariant(number));
            if (order != null)
            {
                WorkContext.CurrentOrder = order;
            }
            return View("customers/order", WorkContext);
        }

        [HttpGet]
        public ActionResult GetAddresses()
        {
            return View("customers/addresses", WorkContext);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View("customers/register", WorkContext);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register([FromForm] Register formModel)
        {
            var user = formModel.ToUser();
            user.StoreId = WorkContext.CurrentStore.Id;

            var result = await _signInManager.UserManager.CreateAsync(user, formModel.Password);
            if (result.Succeeded == true)
            {
                user = await _signInManager.UserManager.FindByNameAsync(user.UserName);
                await _publisher.Publish(new UserRegisteredEvent(WorkContext, user, formModel.ToUserRegistrationInfo()));
                await _signInManager.SignInAsync(user, isPersistent: true);
                await _publisher.Publish(new UserLoginEvent(WorkContext, user));
                return StoreFrontRedirect("~/account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("form", error.Description);
                }
            }

            return View("customers/register", WorkContext);
        }

        [Authorize(Policy = "CanImpersonate")]
        public async Task<IActionResult> ImpersonateUser(string userId)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(User.Identity.Name);
            var impersonatedUser = await _signInManager.UserManager.FindByIdAsync(userId);
            impersonatedUser.OperatorUserId = user.Id;
            impersonatedUser.OperatorUserName = user.UserName;

            // sign out the current user
            await _signInManager.SignOutAsync();

            await _signInManager.SignInAsync(impersonatedUser, isPersistent: false);

            return View("index");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View("customers/login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([FromForm] Login login, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(login.Username))
            {
                ModelState.AddModelError("user_name", "must not be empty");
            }

            if (string.IsNullOrWhiteSpace(login.Password))
            {
                ModelState.AddModelError("password", "must not be empty");
            }

            if (!ModelState.IsValid)
            {
                return View("customers/login", WorkContext);
            }

            var loginResult = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, lockoutOnFailure: true);

            if (loginResult.Succeeded)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(login.Username);

                //Check that current user can sing in to current store
                if (user.AllowedStores.IsNullOrEmpty() || user.AllowedStores.Any(x => x.EqualsInvariant(WorkContext.CurrentStore.Id)))
                {
                    await _publisher.Publish(new UserLoginEvent(WorkContext, user));
                    return StoreFrontRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("form", "User cannot login to current store.");
                }

            }
            //TODO: Locked out not work. Need to add some API methods to support lockout data.
            if (loginResult.IsLockedOut)
            {
                return View("lockedout", WorkContext);
            }

            if (loginResult.RequiresTwoFactor)
            {
                return StoreFrontRedirect("~/account/sendcode");
            }

            ModelState.AddModelError("form", "Login attempt failed.");
            return View("customers/login", WorkContext);

        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return StoreFrontRedirect("~/");
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string authType, string returnUrl)
        {
            if (string.IsNullOrEmpty(authType))
            {
                return new BadRequestResult();
            }

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(authType, Url.Action("ExternalLoginCallback", "Account"));
            return Challenge(properties, authType);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            var currentUser = WorkContext.CurrentUser;
            if (loginInfo == null)
            {
                return View("customers/login", WorkContext);
            }
            User user = null;
            // Sign in the user with this external login provider if the user already has a login.
            var externalLoginResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (!externalLoginResult.Succeeded)
            {
                //TODO: Locked out not work. Need to add some API methods to support lockout data.
                if (externalLoginResult.IsLockedOut)
                {
                    return View("lockedout", WorkContext);
                }

                IdentityResult identityResult;

                if (currentUser.IsRegisteredUser == true)
                {
                    identityResult = await _signInManager.UserManager.AddLoginAsync(currentUser, loginInfo);
                }
                else
                {
                    user = new User()
                    {
                        Email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                        UserName = $"{loginInfo.LoginProvider}--{loginInfo.ProviderKey}",
                        StoreId = WorkContext.CurrentStore.Id,
                    };
                    user.ExternalLogins.Add(new ExternalUserLoginInfo { ProviderKey = loginInfo.ProviderKey, LoginProvider = loginInfo.LoginProvider });
                    identityResult = await _signInManager.UserManager.CreateAsync(user);
                }

                if (!identityResult.Succeeded)
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("customers/login", WorkContext);
                }
            }

            user = await _signInManager.UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (!externalLoginResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                var registrationInfo = new UserRegistrationInfo
                {
                    FirstName = loginInfo.Principal.FindFirstValue(ClaimTypes.GivenName),
                    LastName = loginInfo.Principal.FindFirstValue(ClaimTypes.Surname),
                    UserName = user.UserName,
                    Email = user.Email
                };
                await _publisher.Publish(new UserRegisteredEvent(WorkContext, user, registrationInfo));
            }
            await _publisher.Publish(new UserLoginEvent(WorkContext, user));

            return StoreFrontRedirect(returnUrl);
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View("customers/forgot_password", WorkContext);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPassword formModel)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(formModel.Email);
            if (user != null)
            {
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { UserId = user.Id, Code = "token" }, protocol: Request.Scheme);
                //TODO: Need to change storefront security API to support to do reset password token generation  via ASP.NET UserManager 
                await _commerceCoreApi.GenerateResetPasswordTokenAsync(user.Id, WorkContext.CurrentStore.Id, WorkContext.CurrentLanguage.CultureName, callbackUrl);
            }
            else
            {
                ModelState.AddModelError("form", "User not found");
            }

            return View("customers/forgot_password", WorkContext);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string code, string userId)
        {
            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(userId))
            {
                WorkContext.ErrorMessage = "Error in URL format";

                return View("error", WorkContext);
            }

            var user = await _signInManager.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                WorkContext.ErrorMessage = "User was not found.";
                return View("error", WorkContext);
            }
            WorkContext.ResetPassword = new ResetPassword
            {
                Token = code,
                Email = user.Email
            };
            return View("customers/reset_password", WorkContext);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPassword formModel)
        {
            if (formModel.Email == null || formModel.Token == null)
            {
                WorkContext.ErrorMessage = "Not enough info for reseting password";
                return View("error", WorkContext);
            }

            var user = await _signInManager.UserManager.FindByEmailAsync(formModel.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");

            }
            var result = await _signInManager.UserManager.ResetPasswordAsync(user, formModel.Token, formModel.Password);

            if (result.Succeeded == true)
            {
                return View("customers/reset_password_confirmation", WorkContext);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("customers/reset_password", WorkContext);
        }


        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePassword formModel)
        {
            var result = await _signInManager.UserManager.ChangePasswordAsync(WorkContext.CurrentUser, formModel.OldPassword, formModel.NewPassword);

            if (result.Succeeded == true)
            {
                return StoreFrontRedirect("~/account");
            }
            else
            {
                ModelState.AddModelError("form", result.Errors.FirstOrDefault()?.Description);
                return View("customers/account", WorkContext);
            }
        }
    }
}
