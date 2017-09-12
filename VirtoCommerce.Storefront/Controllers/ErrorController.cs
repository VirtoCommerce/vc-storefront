using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Errors(string errCode)
        {          
            return View(errCode);
        }        
    }
}
