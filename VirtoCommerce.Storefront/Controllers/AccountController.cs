using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;

namespace VirtoCommerce.Storefront.Controllers
{

    public class AccountController : StorefrontControllerBase
    {
        private readonly SignInManager<CustomerInfo> _signInManager;
        public AccountController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, SignInManager<CustomerInfo> signInManager)
            : base(workContextAccessor, urlBuilder)
        {
            _signInManager = signInManager;
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
            var user = new CustomerInfo
            {
                Email = formModel.Email,
                UserName = formModel.UserName,
                StoreId = WorkContext.CurrentStore.Id
            };
            var result = await _signInManager.UserManager.CreateAsync(user, formModel.Password);
            if (result.Succeeded == true)
            {
                //TODO:
                //Publish user login event 
                //await _userLoginEventPublisher.PublishAsync(new UserLoginEvent(WorkContext, WorkContext.CurrentCustomer, customer));

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
                var userInfo = await _signInManager.UserManager.FindByNameAsync(login.Username);
                //Check that it's login on behalf request           
                var onBehalfUserId = Request.Cookies[StorefrontConstants.LoginOnBehalfUserIdCookie];
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
                if (userInfo.AllowedStores.IsNullOrEmpty() || userInfo.AllowedStores.Any(x => x.EqualsInvariant(WorkContext.CurrentStore.Id)))
                {
    
                    //TODO: Publish user login event
                    //Publish user login event 
                    //await _userLoginEventPublisher.PublishAsync(new UserLoginEvent(WorkContext, WorkContext.CurrentCustomer, customer));
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

    }    
}
