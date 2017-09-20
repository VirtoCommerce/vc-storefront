using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Quote;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiAccountController : StorefrontControllerBase
    {
        private readonly SignInManager<CustomerInfo> _signInManager;

        public ApiAccountController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, SignInManager<CustomerInfo> signInManager)
            : base(workContextAccessor, urlBuilder)
        {
            _signInManager = signInManager;
        }

        // GET: storefrontapi/account
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetCurrentCustomer()
        {
            return Json(WorkContext.CurrentCustomer);
        }

        // GET: storefrontapi/account/quotes
        [HttpGet]
        public ActionResult GetCustomerQuotes(int pageNumber, int pageSize, IEnumerable<SortInfo> sortInfos)
        {
            var entries = WorkContext.CurrentCustomer.QuoteRequests;
            entries.Slice(pageNumber, pageSize, sortInfos);
            var retVal = new StaticPagedList<QuoteRequest>(entries.Select(x => x), entries);

            return Json(new
            {
                Results = retVal,
                TotalCount = retVal.TotalItemCount
            });
        }

        // POST: storefrontapi/account
        [HttpPost]
        public async Task<ActionResult> UpdateAccount([FromBody] CustomerInfo customer)
        {
            customer.Id = WorkContext.CurrentCustomer.Id;
        
            await _signInManager.UserManager.UpdateAsync(customer);

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

            var result = await _signInManager.UserManager.ChangePasswordAsync(WorkContext.CurrentCustomer, formModel.OldPassword, formModel.NewPassword);

            return Json(result);
        }

        // POST: storefrontapi/account/addresses
        [HttpPost]
        public async Task<ActionResult> UpdateAddresses([FromBody] ICollection<Address> addresses)
        {
            var contact = WorkContext.CurrentCustomer;
            if (contact != null)
            {
                contact.Addresses.Clear();
                contact.DynamicProperties = null;
                if (addresses != null)
                    contact.Addresses.AddRange(addresses);

                await _signInManager.UserManager.UpdateAsync(contact);
            }

            return Ok();
        }
    }
}