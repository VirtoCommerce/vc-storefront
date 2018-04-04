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
using VirtoCommerce.Storefront.Domain.Security.Notifications;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Security;

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

        // POST: storefrontapi/account/invitation
        [HttpPost]
        public async Task<ActionResult> CreateUserInvitation([FromBody] UserInvitation invitation)
        {
            var result = IdentityResult.Success;
            //TODO: Authorization check
            //TODO: Implement case for invite already exist user to organization
            //Allow to invite only in own organization
            invitation.OrganizationId = WorkContext.CurrentUser?.Contact?.Value?.Organization?.Id;
            var user = await _signInManager.UserManager.FindByEmailAsync(invitation.Email);

            if (user == null)
            {
                user = new User
                {
                    UserName = invitation.UserName ?? invitation.Email,
                    StoreId = WorkContext.CurrentStore.Id,
                    Roles = invitation.Roles,
                    Email = invitation.Email,
                };
                result = await _signInManager.UserManager.CreateAsync(user);
            }
          
            if (result.Succeeded)
            {
                user = await _signInManager.UserManager.FindByNameAsync(user.UserName);
                var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmInvitation", "Account", new { invitation.OrganizationId, user.Email, Token = token }, Request.Scheme);
                var inviteNotification = new RegistrationInvitationNotification(WorkContext.CurrentStore.Id, WorkContext.CurrentLanguage)
                {
                    InviteUrl = callbackUrl,
                    Sender = WorkContext.CurrentStore.Email,
                    Recipient = user.Email
                };
                var sendingResult = await _platformNotificationApi.SendNotificationAsync(inviteNotification.ToNotificationDto());
                if (sendingResult.IsSuccess != true)
                {
                    result = IdentityResult.Failed(new IdentityError() { Description = sendingResult.ErrorMessage });
                }
            }
            return Json(result);
        }

        // GET: storefrontapi/account/organization
        [HttpGet]
        public ActionResult GetCustomerOrganization()
        {
            return Json(WorkContext.CurrentUser?.Contact?.Value?.Organization);
        }

        // POST: storefrontapi/account/organization/contacts/search
        [HttpPost]
        public async Task<ActionResult> GetCustomerOrganizationContactsAsync([FromBody] OrganizationContactsSearchCriteria searchCriteria)
        {
            //TODO: Add authorization checks
            //Allow search only in own organization
            searchCriteria.OrganizationId = WorkContext.CurrentUser?.Contact?.Value?.Organization?.Id;
            if (searchCriteria.OrganizationId != null)
            {
                var result = await _memberService.SearchOrganizationContactsAsync(searchCriteria);
                return Json(result);
            }
            return Ok();
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
