using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.AutoRestClients.NotificationsModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.NotificationsModuleApi.Models;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Domain.Common;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Domain.Security.Notifications;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Common.Notifications;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Security.Events;
using VirtoCommerce.Storefront.Model.Security.Specifications;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("account")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiAccountController : StorefrontControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IEventPublisher _publisher;
        private readonly StorefrontOptions _options;
        private readonly INotifications _platformNotificationApi;
        private readonly IdentityOptions _identityOptions;

        public ApiAccountController(IWorkContextAccessor workContextAccessor,
            IStorefrontUrlBuilder urlBuilder,
            SignInManager<User> signInManager,
            IEventPublisher publisher,
            INotifications platformNotificationApi,
            IOptions<StorefrontOptions> options,
            IOptions<IdentityOptions> identityOptions)
            : base(workContextAccessor, urlBuilder)
        {
            _signInManager = signInManager;
            _publisher = publisher;
            _options = options.Value;
            _platformNotificationApi = platformNotificationApi;
            _identityOptions = identityOptions.Value;
        }

        // GET: storefrontapi/account
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<User> GetCurrentUser()
        {
            return WorkContext.CurrentUser;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserActionIdentityResult>> Login([FromBody] Login login, [FromQuery] string returnUrl)
        {
            //TODO: Two factor auth
            var result = UserActionIdentityResult.Success;
            TryValidateModel(login);

            if (!ModelState.IsValid)
            {
                return UserActionIdentityResult.Failed(ModelState.Values.SelectMany(x => x.Errors)
                      .Select(x => new IdentityError { Description = x.ErrorMessage })
                      .ToArray());
            }
            login.UserName = login.UserName?.Trim();

            var user = await _signInManager.UserManager.FindByNameAsync(login.UserName);

            if (user == null)
            {
                result = UserActionIdentityResult.Failed(SecurityErrorDescriber.LoginFailed());
            }
            else if (!new CanUserLoginToStoreSpecification(user).IsSatisfiedBy(WorkContext.CurrentStore))
            {
                result = UserActionIdentityResult.Failed(SecurityErrorDescriber.UserCannotLoginInStore());
            }
            else if (new IsUserLockedOutSpecification().IsSatisfiedBy(user))
            {
                result = UserActionIdentityResult.Failed(SecurityErrorDescriber.UserIsLockedOut());
            }
            else if (new IsUserSuspendedSpecification().IsSatisfiedBy(user))
            {
                result = UserActionIdentityResult.Failed(SecurityErrorDescriber.AccountIsBlocked());
            }
            else
            {
                var loginResult = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, lockoutOnFailure: true);

                if (!loginResult.Succeeded)
                {
                    result = UserActionIdentityResult.Failed(SecurityErrorDescriber.LoginFailed());
                }
                else
                {
                    await _publisher.Publish(new UserLoginEvent(WorkContext, user));
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        var newUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : "~/";
                        result.ReturnUrl = UrlBuilder.ToAppRelative(newUrl, WorkContext.CurrentStore, WorkContext.CurrentLanguage);
                    }
                }
            }
            return result;
        }

        // POST: storefrontapi/account/user
        [HttpPost("user")]
        public async Task<ActionResult<UserActionIdentityResult>> RegisterUser([FromBody] UserRegistration registration)
        {
            var result = UserActionIdentityResult.Success;

            TryValidateModel(registration);

            if (!ModelState.IsValid)
            {
                return UserActionIdentityResult.Failed(ModelState.Values.SelectMany(x => x.Errors)
                       .Select(x => new IdentityError { Description = x.ErrorMessage })
                       .ToArray());
            }
            // Register user
            var user = registration.ToUser();
            user.Contact = registration.ToContact();
            user.StoreId = WorkContext.CurrentStore.Id;

            var identityResult = await _signInManager.UserManager.CreateAsync(user, registration.Password);
            if (identityResult.Succeeded)
            {
                user = await _signInManager.UserManager.FindByNameAsync(user.UserName);
                await _publisher.Publish(new UserRegisteredEvent(WorkContext, user, registration));

                if (!_identityOptions.SignIn.RequireConfirmedEmail)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    await _publisher.Publish(new UserLoginEvent(WorkContext, user));
                }

                // Send new user registration notification
                var registrationEmailNotification = new RegistrationEmailNotification(WorkContext.CurrentStore.Id, WorkContext.CurrentLanguage)
                {
                    FirstName = registration.FirstName,
                    LastName = registration.LastName,
                    Login = registration.UserName,
                    Sender = WorkContext.CurrentStore.Email,
                    Recipient = GetUserEmail(user)
                };
                await SendNotificationAsync(registrationEmailNotification);

                if (_options.SendAccountConfirmation)
                {
                    var token = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { UserId = user.Id, Token = token }, protocol: Request.Scheme, host: WorkContext.CurrentStore.Host);
                    var emailConfirmationNotification = new EmailConfirmationNotification(WorkContext.CurrentStore.Id, WorkContext.CurrentLanguage)
                    {
                        Url = callbackUrl,
                        Sender = WorkContext.CurrentStore.Email,
                        Recipient = GetUserEmail(user)
                    };
                    var sendNotifcationResult = await SendNotificationAsync(emailConfirmationNotification);
                    if (sendNotifcationResult.IsSuccess == false)
                    {
                        var error = SecurityErrorDescriber.ErrorSendNotification(sendNotifcationResult.ErrorMessage);
                        result = UserActionIdentityResult.Failed(new IdentityError { Code = error.Code, Description = error.Description });
                    }
                }
            }
            else
            {
                result = UserActionIdentityResult.Failed(identityResult.Errors.ToArray());

            }

            return result;
        }


        // POST: storefrontapi/account/password
        [HttpPost("password")]
        public async Task<ActionResult<PasswordChangeResult>> ChangePassword([FromBody] ChangePassword formModel)
        {
            var result = await _signInManager.UserManager.ChangePasswordAsync(WorkContext.CurrentUser, formModel.OldPassword, formModel.NewPassword);
            return new PasswordChangeResult { Succeeded = result.Succeeded, Errors = result.Errors.Select(x => new FormError { Code = x.Code.PascalToKebabCase(), Description = x.Description }).ToList() };
        }


        // DELETE: storefrontapi/account/phonenumber
        [HttpDelete("phonenumber")]
        public async Task<ActionResult<RemovePhoneNumberResult>> RemovePhoneNumber()
        {
            var twoFactorAuthEnabled = await _signInManager.UserManager.GetTwoFactorEnabledAsync(WorkContext.CurrentUser);
            if (twoFactorAuthEnabled)
            {
                return Forbid();
            }

            var result = await _signInManager.UserManager.SetPhoneNumberAsync(WorkContext.CurrentUser, null);
            await _signInManager.SignInAsync(WorkContext.CurrentUser, isPersistent: false);

            return new RemovePhoneNumberResult { Succeeded = result.Succeeded, Errors = result.Errors.Select(x => x.Description) };
        }

        // POST: storefrontapi/account/twofactorauthentification
        [HttpPost("twofactorauthentification")]
        public async Task<ActionResult<ChangeTwoFactorAuthenticationResult>> ChangeTwoFactorAuthentication([FromBody] ChangeTwoFactorAuthenticationModel model)
        {
            if (model.Enabled)
            {
                var phoneConfirmed = await _signInManager.UserManager.IsPhoneNumberConfirmedAsync(WorkContext.CurrentUser);
                if (!phoneConfirmed)
                {
                    var url = "/account/phonenumber";

                    return new ChangeTwoFactorAuthenticationResult { Succeeded = false, VerificationUrl = url };
                }
            }

            var result = await _signInManager.UserManager.SetTwoFactorEnabledAsync(WorkContext.CurrentUser, model.Enabled);
            await _signInManager.SignInAsync(WorkContext.CurrentUser, isPersistent: false);

            return new ChangeTwoFactorAuthenticationResult { Succeeded = result.Succeeded, Errors = result.Errors.Select(x => x.Description) };
        }

        // POST: storefrontapi/account/phonenumber
        [HttpPost("phonenumber")]
        public async Task<ActionResult<UpdatePhoneNumberResult>> UpdatePhoneNumber([FromBody] UpdatePhoneNumberModel model)
        {
            var twoFactorAuthEnabled = await _signInManager.UserManager.GetTwoFactorEnabledAsync(WorkContext.CurrentUser);

            if (twoFactorAuthEnabled)
            {
                return Forbid();
            }

            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                return new UpdatePhoneNumberResult { Succeeded = false, Error = "Phone number is not valid" };
            }

            var result = await _signInManager.UserManager.SetPhoneNumberAsync(WorkContext.CurrentUser, model.PhoneNumber);
            await _signInManager.SignInAsync(WorkContext.CurrentUser, isPersistent: false);

            return new UpdatePhoneNumberResult { Succeeded = result.Succeeded };

        }

        private static string GetUserEmail(User user)
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


        private async Task<NotificationSendResult> SendNotificationAsync(NotificationBase notification)
        {
            NotificationSendResult result;

            try
            {
                result = await _platformNotificationApi.SendNotificationByRequestAsync(notification.ToNotificationDto());
            }
            catch (Exception exception)
            {
                result = new NotificationSendResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error occurred while sending notification: {exception.Message}"
                };
            }

            return result;
        }
    }
}
