using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VirtoCommerce.Storefront.Tests.Routing.Infrastructure
{
    /// <summary>
    /// Test filter that helps to test application routing. It short-circuits request processing
    /// and returns routing data instead of actual action result, so the target controller method 
    /// does not actually get executed.
    /// </summary>
    public class RoutingTestingActionFilter : IActionFilter
    {
        /// <inheritdoc />
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var requestPath = context.HttpContext.Request.Path;
            var controllerTypeName = context.Controller.GetType().FullName;
            var methodName = context.ActionDescriptor.DisplayName;
            var arguments = context.ActionArguments;
            var routingDataResult = new RoutingDataResult(requestPath, controllerTypeName, methodName, arguments);

            context.Result = new JsonResult(routingDataResult);
        }

        /// <inheritdoc />
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Actual action does not get executed, so body of this method intentionally left blank.
        }
    }
}
