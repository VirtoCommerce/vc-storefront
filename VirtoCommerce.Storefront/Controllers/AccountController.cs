using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Events;

namespace VirtoCommerce.Storefront.Controllers
{
    [Authorize]
    public class AccountController : StorefrontControllerBase
    {
        private readonly SignInManager<CustomerInfo> _signInManager;
        private readonly IEventPublisher _publisher;
        public AccountController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, SignInManager<CustomerInfo> signInManager, IEventPublisher publisher)
            : base(workContextAccessor, urlBuilder)
        {
            _signInManager = signInManager;
            _publisher = publisher;
        }

        //GET: /account
        [HttpGet]
        public ActionResult GetAccount()
        {
            //Customer should be already populated in WorkContext middle-ware
            return View("customers/account", WorkContext);
        }

        //POST: /account
        [HttpPost]
        public async Task<ActionResult> UpdateAccount(CustomerInfo customer)
        {
            //Do not allow to update other accounts
            customer.Id = WorkContext.CurrentCustomer.Id;

            await _signInManager.UserManager.UpdateAsync(customer);
            return View("customers/account", WorkContext);
        }

       

        [HttpGet]
        public ActionResult GetOrderDetails(string number)
        {
            var order = WorkContext.CurrentCustomer.Orders.FirstOrDefault(x => x.Number.EqualsInvariant(number));
            if (order != null)
            {
                WorkContext.CurrentOrder = order;
                return View("customers/order", WorkContext);
            }
            return NotFound();
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
        public async Task<ActionResult> Register([FromForm] Register formModel)
        {
            var user = formModel.ToCustomerInfo();
            user.StoreId = WorkContext.CurrentStore.Id;
         
            var result = await _signInManager.UserManager.CreateAsync(user, formModel.Password);
            if (result.Succeeded == true)
            {
                await _publisher.Publish(new UserRegisteredEvent(WorkContext, user));
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string userId)
        {
            return View("customers/login");
        }
    
        [HttpPost]
        [AllowAnonymous]
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
                //Check that it's login on behalf request           
                //var onBehalfUserId = Request.Cookies[StorefrontConstants.LoginOnBehalfUserIdCookie];
                //TODO: login on behalf
                //if (!string.IsNullOrEmpty(onBehalfUserId) && !string.Equals(onBehalfUserId, User.Identity.Name) && await _customerService.CanLoginOnBehalfAsync(WorkContext.CurrentStore.Id, customer.UserId))
                //{
                //    var userOnBehalf = await _commerceCoreApi.StorefrontSecurity.GetUserByIdAsync(onBehalfUserId);
                //    if (userOnBehalf != null)
                //    {
                //        var customerOnBehalf = await GetStorefrontCustomerByUserAsync(userOnBehalf);

                //        customerOnBehalf.OperatorUserId = customer.UserId;
                //        customerOnBehalf.OperatorUserName = customer.UserName;
                //        //change the operator login on the customer login
                //        customer = customerOnBehalf;
                //        //Clear LoginOnBehalf cookies
                //        SetUserIdForLoginOnBehalf(Response, null);
                //    }
                //    // TODO: Configure the reduced login expiration
                //}

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
            if (loginInfo == null)
            {
                return new BadRequestResult();
            }

            CustomerInfo customer = await _signInManager.UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (customer == null)
            {
                customer = new CustomerInfo()
                {
                    FullName = loginInfo.Principal.Identity.Name,
                    UserName = string.Join("--", loginInfo.LoginProvider, loginInfo.ProviderKey),
                    StoreId = WorkContext.CurrentStore.Id,
                };

                var result = await _signInManager.UserManager.AddLoginAsync(customer, loginInfo);
                if (!result.Succeeded)
                {
                    return new StatusCodeResult((int)System.Net.HttpStatusCode.InternalServerError);
                }
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                await _publisher.Publish(new UserLoginEvent(WorkContext, customer));
            }

            return StoreFrontRedirect(returnUrl);
        }
    }    
}
