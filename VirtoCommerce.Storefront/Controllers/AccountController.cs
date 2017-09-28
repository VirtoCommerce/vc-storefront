using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
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
        private readonly IAuthorizationService _authorizationService;
        public AccountController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, SignInManager<CustomerInfo> signInManager, IEventPublisher publisher, IAuthorizationService authorizationService)
            : base(workContextAccessor, urlBuilder)
        {
            _signInManager = signInManager;
            _publisher = publisher;
            _authorizationService = authorizationService;
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

        [Authorize(Policy = "CanImpersonate")]
        [HttpGet]
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
    }    
}
