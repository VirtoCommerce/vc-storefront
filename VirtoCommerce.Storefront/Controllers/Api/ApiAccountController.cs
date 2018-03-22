using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiAccountController : StorefrontControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IMemberService _memberService;
        public ApiAccountController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, SignInManager<User> signInManager, IMemberService memberService)
            : base(workContextAccessor, urlBuilder)
        {
            _signInManager = signInManager;
            _memberService = memberService;
        }

        // GET: storefrontapi/account
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetCurrentCustomer()
        {       
            return Json(WorkContext.CurrentUser);
        }

        // GET: storefrontapi/account/quotes
        [HttpGet]
        public ActionResult GetCustomerQuotes(int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos)
        {
            if (WorkContext.CurrentUser.IsRegisteredUser)
            {
                var entries = WorkContext.CurrentUser?.QuoteRequests;
                if (entries != null)
                {
                    entries.Slice(pageNumber, pageSize, sortInfos);
                    var retVal = new StaticPagedList<QuoteRequest>(entries.Select(x => x), entries);

                    return Json(new
                    {
                        Results = retVal,
                        TotalCount = retVal.TotalItemCount
                    });
                }
            }
            return NoContent();
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

            return Json(new { Succeeded = result.Succeeded, Errors = result.Errors.Select(x => x.Description) });
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
