using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Domain.Common;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Domain.Security.Notifications;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Security.Events;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiAccountController : StorefrontControllerBase
    {
        private readonly IEventPublisher _publisher;
        private readonly SignInManager<User> _signInManager;
        private readonly IMemberService _memberService;
        private readonly IStorefrontSecurity _commerceCoreApi;
        private readonly INotifications _platformNotificationApi;

        public ApiAccountController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, SignInManager<User> signInManager,
                                    IMemberService memberService, IEventPublisher publisher, IStorefrontSecurity commerceCoreApi, INotifications platformNotificationApi)
            : base(workContextAccessor, urlBuilder)
        {
            _signInManager = signInManager;
            _memberService = memberService;
            _publisher = publisher;
            _commerceCoreApi = commerceCoreApi;
            _platformNotificationApi = platformNotificationApi;
        }

        // GET: storefrontapi/account
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetCurrentCustomer()
        {       
            return Json(WorkContext.CurrentUser);
        }

        // DELETE: storefrontapi/account/{userName}
        [HttpDelete]
        public async Task<ActionResult> DeleteUser([FromRoute] string userName)
        {
            //TODO: Authorization check
            var result = IdentityResult.Success;
            var user = await _signInManager.UserManager.FindByNameAsync(userName);
            if(user != null)
            {
                result = await _signInManager.UserManager.DeleteAsync(user);
                if(result.Succeeded)
                {
                   await _publisher.Publish(new UserDeletedEvent(WorkContext, user));
                }
            }
            return Json(result);
        }

        // POST: storefrontapi/account/user
        [HttpPost]
        public async Task<ActionResult> RegisterNewUser([FromBody] UserRegistration registration)
        {
            var result = IdentityResult.Success;

            TryValidateModel(registration);
            //TODO: Authorization check
            //Allow to register new users within own organization
            registration.OrganizationId = WorkContext.CurrentUser?.Contact?.Value?.Organization?.Id;
            if (ModelState.IsValid)
            {
                var user = registration.ToUser();
                user.StoreId = WorkContext.CurrentStore.Id;
                
                result = await _signInManager.UserManager.CreateAsync(user, registration.Password);
                if (result.Succeeded == true)
                {
                    user = await _signInManager.UserManager.FindByNameAsync(user.UserName);
                    await _publisher.Publish(new UserRegisteredEvent(WorkContext, user, registration));
                }               
            }
            else
            {
                result = IdentityResult.Failed(ModelState.Values.SelectMany(x => x.Errors).Select(x => new IdentityError { Description = x.ErrorMessage }).ToArray());
            }
            return Json(result);
        }

        // POST: storefrontapi/account/invitation
        [HttpPost]
        public async Task<ActionResult> CreateUserInvitation([FromBody] UsersInvitation invitation)
        {
            var result = IdentityResult.Success;
            //TODO: Authorization check
            //TODO: Implement case for invite already exist user to organization
            //Allow to invite only in own organization
            var organizationId = WorkContext.CurrentUser?.Contact?.Value?.Organization?.Id;

            foreach (var email in invitation.Emails)
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = email,
                        StoreId = WorkContext.CurrentStore.Id,
                        Roles = invitation.Roles,
                        Email = email,
                    };
                    result = await _signInManager.UserManager.CreateAsync(user);
                }
                if (result.Succeeded)
                {
                    user = await _signInManager.UserManager.FindByNameAsync(user.UserName);
                    var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmInvitation", "Account", new { OrganizationId =  organizationId, user.Email, Token = token }, Request.Scheme);
                    var inviteNotification = new RegistrationInvitationNotification(WorkContext.CurrentStore.Id, WorkContext.CurrentLanguage)
                    {
                        InviteUrl = callbackUrl,
                        Sender = WorkContext.CurrentStore.Email,
                        Recipient = user.Email
                    };
                    var sendingResult = await _platformNotificationApi.SendNotificationAsync(inviteNotification.ToNotificationDto());
                    if (sendingResult.IsSuccess != true)
                    {
                        var errors = result.Errors.Concat(new IdentityError[] { new IdentityError() { Description = sendingResult.ErrorMessage } }).ToArray();
                        result = IdentityResult.Failed(errors);
                    }
                }
            }
            return Json(result);
        }

        // PUT: storefrontapi/account/organization
        [HttpPut]
        public async Task<ActionResult> UpdateCustomerOrganization([FromBody] Organization organization)
        {
            //TODO: Add authorization checks
            var userOrganizationId = WorkContext.CurrentUser?.Contact?.Value?.Organization?.Id;
            //Allow update only organization which user belongs to
            if (organization.Id == userOrganizationId)
            {
                await _memberService.UpdateOrganizationAsync(organization);
            }
            return Ok();
        }

        // GET: storefrontapi/account/organization
        [HttpGet]
        public ActionResult GetCustomerOrganization()
        {
            return Json(WorkContext.CurrentUser?.Contact?.Value?.Organization);
        }

        // POST: storefrontapi/account/organization/contacts/search
        [HttpPost]
        public async Task<ActionResult> SearchCustomerOrganizationContactsAsync([FromBody] OrganizationContactsSearchCriteria searchCriteria)
        {
            //TODO: Add authorization checks
            //Allow search only in own organization
            searchCriteria.OrganizationId = WorkContext.CurrentUser?.Contact?.Value?.Organization?.Id;
            if (searchCriteria.OrganizationId != null)
            {
                var result = await _memberService.SearchOrganizationContactsAsync(searchCriteria);
                return Json(new { TotalCount = result.TotalItemCount, Results = result.ToArray() });
            }
            return Ok();
        }

        // POST: storefrontapi/account/{userName}/lock
        [HttpPost]
        public async Task<ActionResult> LockUser([FromRoute]string userName)
        {
            //TODO: Add authorization checks
            var result = IdentityResult.Success;
            var user = await _signInManager.UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                await _signInManager.UserManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }
            return Json(result);
        }

        // POST: storefrontapi/account/{userName}/unlock
        [HttpPost]
        public async Task<ActionResult> UnlockUser([FromRoute] string userName)
        {
            //TODO: Add authorization checks
            var result = IdentityResult.Success;
            var user = await _signInManager.UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                await _signInManager.UserManager.ResetAccessFailedCountAsync(user);
                await _signInManager.UserManager.SetLockoutEndDateAsync(user, DateTimeOffset.MinValue);
            }
            return Json(result);
        }

        // POST: storefrontapi/account
        [HttpPost]
        public async Task<ActionResult> UpdateAccount([FromBody] ContactUpdateInfo updateInfo)
        {
            await _memberService.UpdateContactAsync(WorkContext.CurrentUser.ContactId, updateInfo);

            return Ok();
        }

        // POST: storefrontapi/account/password
        [HttpPost]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePassword formModel)
        {
            var changePassword = new ChangePasswordInfo
            {
                OldPassword = formModel.OldPassword,
                NewPassword = formModel.NewPassword,
            };

            var result = await _signInManager.UserManager.ChangePasswordAsync(WorkContext.CurrentUser, formModel.OldPassword, formModel.NewPassword);

            return Json(new {  result.Succeeded, Errors = result.Errors.Select(x => x.Description) });
        }

        // POST: storefrontapi/account/addresses
        [HttpPost]
        public async Task<ActionResult> UpdateAddresses([FromBody] IList<Address> addresses)
        {
            await _memberService.UpdateContactAddressesAsync(WorkContext.CurrentUser.ContactId, addresses);

            return Ok();
        }
    }
}
