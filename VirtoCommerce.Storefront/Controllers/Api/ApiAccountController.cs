using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
            TryValidateModel(login);

            if (!ModelState.IsValid)
            {
                return UserActionIdentityResult.Failed(ModelState.Values.SelectMany(x => x.Errors)
                      .Select(x => new IdentityError { Description = x.ErrorMessage })
                      .ToArray());
            }

            login.UserName = login.UserName?.Trim();

            var user = await _signInManager.UserManager.FindByNameAsync(login.UserName);
            var result = CheckLoginUser(user);

            if (result != UserActionIdentityResult.Success)
            {
                return result;
            }

            var loginResult = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, lockoutOnFailure: true);

            if (!loginResult.Succeeded)
            {
                result = UserActionIdentityResult.Failed(SecurityErrorDescriber.LoginFailed());
            }
            else
            {
                await _publisher.Publish(new UserLoginEvent(WorkContext, user));
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return result;
                }

                var newUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : "~/";
                result.ReturnUrl = UrlBuilder.ToAppRelative(newUrl, WorkContext.CurrentStore, WorkContext.CurrentLanguage);
            }
            return result;
        }

        private UserActionIdentityResult CheckLoginUser(User user)
        {
            if (user == null)
            {
                return UserActionIdentityResult.Failed(SecurityErrorDescriber.LoginFailed());
            }

            if (!new CanUserLoginToStoreSpecification(user).IsSatisfiedBy(WorkContext.CurrentStore))
            {
                return UserActionIdentityResult.Failed(SecurityErrorDescriber.UserCannotLoginInStore());
            }

            if (new IsUserLockedOutSpecification().IsSatisfiedBy(user))
            {
                return UserActionIdentityResult.Failed(SecurityErrorDescriber.UserIsLockedOut());
            }

            if (new IsUserSuspendedSpecification().IsSatisfiedBy(user))
            {
                return UserActionIdentityResult.Failed(SecurityErrorDescriber.AccountIsBlocked());
            }

            return UserActionIdentityResult.Success;
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

        // POST: storefrontapi/account/logout
        [HttpGet("logout")]
        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return NoContent();
        }

        // POST: storefrontapi/account/forgotpassword
        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<UserActionIdentityResult>> ForgotPassword([FromBody] ForgotPasswordModel forgotPassword)
        {
            var result = UserActionIdentityResult.Success;

            TryValidateModel(forgotPassword);

            if (!ModelState.IsValid)
            {
                return UserActionIdentityResult.Failed(ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => new IdentityError { Description = x.ErrorMessage })
                    .ToArray());
            }

            var user = await _signInManager.UserManager.FindByEmailAsync(forgotPassword.Email);

            if (user == null)
            {
                user = await _signInManager.UserManager.FindByNameAsync(forgotPassword.Email);
            }

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return result;
            }

            NotificationBase resetPasswordNotification;

            if (_options.ResetPasswordNotificationGateway.EqualsInvariant("Phone"))
            {
                var phoneNumber = await _signInManager.UserManager.GetPhoneNumberAsync(user);

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    return UserActionIdentityResult.Failed(SecurityErrorDescriber.PhoneNumberNotFound());
                }

                var token = await _signInManager.UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, "ResetPassword");

                resetPasswordNotification = new ResetPasswordSmsNotification(WorkContext.CurrentStore.Id, WorkContext.CurrentLanguage)
                {
                    Token = token,
                    Recipient = phoneNumber,
                };
            }
            else // "Email"
            {
                var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user);
                token = WebUtility.UrlEncode(token);
                var resetPasswordUri = new UriBuilder(forgotPassword.ResetPasswordUrl) { Query = $"userId={user.Id}&token={token}" };

                resetPasswordNotification = new ResetPasswordEmailNotification(WorkContext.CurrentStore.Id, WorkContext.CurrentLanguage)
                {
                    Url = resetPasswordUri.ToString(),
                    Sender = WorkContext.CurrentStore.Email,
                    Recipient = GetUserEmail(user)
                };
            }

            var sendingResult = await SendNotificationAsync(resetPasswordNotification);

            if (sendingResult.IsSuccess == false)
            {
                return UserActionIdentityResult.Failed(SecurityErrorDescriber.ErrorSendNotification(sendingResult.ErrorMessage));
            }

            return result;
        }

        // POST: storefrontapi/account/validateToken
        [HttpPost("validateToken")]
        [AllowAnonymous]
        public async Task<ActionResult<UserActionIdentityResult>> ValidateResetPasswordToken([FromBody] ValidateTokenModel model)
        {
            var result = UserActionIdentityResult.Success;

            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                return UserActionIdentityResult.Failed(ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => new IdentityError { Description = x.ErrorMessage })
                    .ToArray());
            }

            var user = await _signInManager.UserManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return UserActionIdentityResult.Failed(SecurityErrorDescriber.InvalidToken());
            }

            var isValidToken = await _signInManager.UserManager.VerifyUserTokenAsync(user, _signInManager.UserManager.Options.Tokens.PasswordResetTokenProvider, UserManager<User>.ResetPasswordTokenPurpose, model.Token);

            if (!isValidToken)
            {
                return UserActionIdentityResult.Failed(SecurityErrorDescriber.InvalidToken());
            }

            return result;
        }

        // POST: storefrontapi/account/resetPassword
        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<UserActionIdentityResult>> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var result = UserActionIdentityResult.Success;

            TryValidateModel(model);

            if (string.IsNullOrEmpty(model.UserId))
            {
                return UserActionIdentityResult.Failed(SecurityErrorDescriber.ResetPasswordInvalidData());
            }

            if (!ModelState.IsValid)
            {
                return UserActionIdentityResult.Failed(ModelState.Values.SelectMany(x => x.Errors)
                    .Select(x => new IdentityError { Description = x.ErrorMessage })
                    .ToArray());
            }

            var user = await _signInManager.UserManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return result;
            }

            var resetPasswordResult = await _signInManager.UserManager.ResetPasswordAsync(user, model.Token, model.Password);


            if (!resetPasswordResult.Succeeded)
            {
                result = UserActionIdentityResult.Failed(resetPasswordResult.Errors.ToArray());
            }

            return result;
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
