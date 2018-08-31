using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Tests.Routing.Infrastructure
{
    /// <summary>
    /// Result of request processing produced by <see cref="RoutingTestingActionFilter"/>.
    /// It contains some data that may be useful to test action routing - 
    /// i.e. requested path, type of controller that should process that request,
    /// name of method that should handle that request, and arguments that were passed
    /// with that request.
    /// </summary>
    public class RoutingDataResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="RoutingDataResult"/>.
        /// </summary>
        /// <param name="requestPath">Path that was requested by client.</param>
        /// <param name="controllerTypeName">Full type name of controller that would process the request.</param>
        /// <param name="controllerMethodName">Fully-qualified name of controller method that would be used
        /// to process request.</param>
        /// <param name="arguments">A name-value dictionary containing arguments passed with the request.</param>
        public RoutingDataResult(string requestPath, string controllerTypeName, string controllerMethodName, IDictionary<string, object> arguments)
        {
            RequestPath = requestPath;
            ControllerTypeName = controllerTypeName;
            ControllerMethodName = controllerMethodName;
            Arguments = arguments;
        }

        /// <summary>
        /// Path that was requested by client.
        /// </summary>
        public string RequestPath { get; }

        /// <summary>
        /// Full type name of controller that would process the request.
        /// </summary>
        public string ControllerTypeName { get; }

        /// <summary>
        /// Fully-qualified name of controller method that would be used to process request.
        /// </summary>
        public string ControllerMethodName { get; }

        /// <summary>
        /// A name-value dictionary containing arguments passed with the request.
        /// </summary>
        public IDictionary<string, object> Arguments { get; }
    }
}
