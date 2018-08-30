using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Tests.Routing.Infrastructure
{
    public class RoutingDataResult
    {
        public RoutingDataResult(string requestPath, string controllerTypeName, string controllerMethodName, IDictionary<string, object> arguments)
        {
            RequestPath = requestPath;
            ControllerTypeName = controllerTypeName;
            ControllerMethodName = controllerMethodName;
            Arguments = arguments;
        }

        public string RequestPath { get; }

        public string ControllerTypeName { get; }

        public string ControllerMethodName { get; }

        public IDictionary<string, object> Arguments { get; }
    }
}
