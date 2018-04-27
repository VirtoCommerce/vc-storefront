using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Domain.Common;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Domain.Security.Notifications;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Security.Events;
using VirtoCommerce.Storefront.Model.Security.Specifications;

namespace VirtoCommerce.Storefront.Controllers
{
    [Authorize]
    public class AccountController : StorefrontControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IEventPublisher _publisher;
        private readonly IStorefrontSecurity _commerceCoreApi;
        private readonly IStorefrontUrlBuilder _urlBuilder;
        private readonly StorefrontOptions _options;
        private readonly INotifications _platformNotificationApi;

        private readonly string[] _firstNameClaims = { ClaimTypes.GivenName, "urn:github:name", ClaimTypes.Name };

        public AccountController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, SignInManager<User> signInManager,
            IEventPublisher publisher, IStorefrontSecurity commerceCoreApi, INotifications platformNotificationApi, IOptions<StorefrontOptions> options)
            : base(workContextAccessor, urlBuilder)
        {
            _signInManager = signInManager;
            _publisher = publisher;
            _commerceCoreApi = commerceCoreApi;
            _urlBuilder = urlBuilder;
            _options = options.Value;
            _platformNotificationApi = platformNotificationApi;
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
            var order = WorkContext.CurrentUser?.Orders.FirstOrDefault(x => x.Number.EqualsInvariant(number));
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
        public async Task<ActionResult> Register([FromForm] UserRegistration registration)
        {
            TryValidateModel(registration);

            if (ModelState.IsValid)
            {
                //Register user
                var user = registration.ToUser();
                user.Contact = registration.ToContact();
                user.StoreId = WorkContext.CurrentStore.Id;

                var result = await _signInManager.UserManager.CreateAsync(user, registration.Password);
                if (result.Succeeded == true)
                {
                    user = await _signInManager.UserManager.FindByNameAsync(user.UserName);
                    await _publisher.Publish(new UserRegisteredEvent(WorkContext, user, registration));
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    await _publisher.Publish(new UserLoginEvent(WorkContext, user));
                    if (_options.SendAccountConfirmation)
                    {
                        var token = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { UserId = user.Id, Token = token }, protocol: Request.Scheme);
                        var emailConfirmationNotification = new EmailConfirmationNotification(WorkContext.CurrentStore.Id, WorkContext.CurrentLanguage)
                        {
                            Url = callbackUrl,
                            Sender = WorkContext.CurrentStore.Email,
                            Recipient = GetUserEmail(user)
                        };
                        await _platformNotificationApi.SendNotificationAsync(emailConfirmationNotification.ToNotificationDto());
                    }
                    return StoreFrontRedirect("~/account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("form", error.Description);
                    }
                }
            }
            WorkContext.Form = registration;
            return View("customers/register", WorkContext);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmInvitation(string organizationId, string email, string token)

        {
            WorkContext.Form = new UserRegistrationByInvitation
            {
                Email = email,
                OrganizationId = organizationId,
                Token = token
            };
            return View("customers/confirm_invitation", WorkContext);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmInvitation([FromForm] UserRegistrationByInvitation register)
        {   
            var result = IdentityResult.Success;
            TryValidateModel(register);

            if (ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(register.Email);
                if (user != null)
                {
                    if(user.UserName != register.UserName)
                    {
                        user.UserName = register.UserName;
                    }
                    result = await _signInManager.UserManager.ResetPasswordAsync(user, register.Token, register.Password);
                    if (result.Succeeded)
                    {
                        user.Contact = register.ToContact();
                        user.Contact.OrganizationId = register.OrganizationId;
                        result = await _signInManager.UserManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            await _publisher.Publish(new UserRegisteredEvent(WorkContext, user, register));
                            await _signInManager.SignInAsync(user, isPersistent: true);
                            await _publisher.Publish(new UserLoginEvent(WorkContext, user));
                        }
                        return StoreFrontRedirect("~/account");
                    }
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            WorkContext.Form = register;
            return View("customers/confirm_invitation", WorkContext);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string token)
        {
            var result = await _signInManager.UserManager.ConfirmEmailAsync(WorkContext.CurrentUser, token);
            if (!result.Succeeded)
            {
                return View("error");
            }
            return View("confirmation-done");
        }

        [Authorize(Policy = CanImpersonateAuthorizationRequirement.PolicyName)]
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
            TryValidateModel(login);
            
            if (!ModelState.IsValid)
            {
                return View("customers/login", WorkContext);
            }

            var loginResult = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, lockoutOnFailure: true);

            if (loginResult.Succeeded)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(login.Username);

                //Check that current user can sing in to current store
                if (new CanUserLoginToStoreSpecification(user).IsSatisfiedBy(WorkContext.CurrentStore))
                {
                    await _publisher.Publish(new UserLoginEvent(WorkContext, user));
                    return StoreFrontRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("form", "User cannot login to current store.");
                }
            }
            if (loginResult.IsLockedOut)
            {
                return View("lockedout", WorkContext);
            }

            if (loginResult.RequiresTwoFactor)
            {
                return StoreFrontRedirect("~/account/sendcode");
            }

            ModelState.AddModelError("form", "Login attempt failed.");
            WorkContext.Form = login;
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
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(authType, Url.Action("ExternalLoginCallback", "Account", new { returnUrl = returnUrl }));
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
                    var userRegistration = new UserRegistration
                    {
                        FirstName = loginInfo.Principal.FindFirstValue(_firstNameClaims, "unknown"),
                        LastName = loginInfo.Principal.FindFirstValue(ClaimTypes.Surname),
                        UserName = user.UserName,
                        Email = user.Email
                    };
                    user.Contact = userRegistration.ToContact();
                    identityResult = await _signInManager.UserManager.CreateAsync(user);
                    if (identityResult.Succeeded)
                    {
                        await _publisher.Publish(new UserRegisteredEvent(WorkContext, user, userRegistration));
                    }
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
                var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { UserId = user.Id, Token = token }, protocol: Request.Scheme);
                var resetPasswordEmailNotification = new ResetPasswordEmailNotification(WorkContext.CurrentStore.Id, WorkContext.CurrentLanguage)
                {
                    Url = callbackUrl,
                    Sender = WorkContext.CurrentStore.Email,
                    Recipient = GetUserEmail(user)
                };
                var sendingResult = await _platformNotificationApi.SendNotificationAsync(resetPasswordEmailNotification.ToNotificationDto());
                if(sendingResult.IsSuccess != true)
                {
                    ModelState.AddModelError("form", sendingResult.ErrorMessage);
                }
            }
            else
            {
                ModelState.AddModelError("form", "User not found");
            }

            return View("customers/forgot_password", WorkContext);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string token, string userId)
        {
            if (string.IsNullOrEmpty(token) && string.IsNullOrEmpty(userId))
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
            WorkContext.Form = new ResetPassword
            {
                Token = token,
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

            if (result.Succeeded)
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

        private string GetUserEmail(User user)
        {
            string email = null;
            if (user != null)
            {
                email = user.Email ?? user.UserName;
                if (user.Contact != null)
                {
                    email = user.Contact?.Email ?? email;
                }
            }
            return email;
        }
    }
}
