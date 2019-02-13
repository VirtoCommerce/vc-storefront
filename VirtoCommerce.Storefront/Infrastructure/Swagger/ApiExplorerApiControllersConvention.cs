using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace VirtoCommerce.Storefront.Infrastructure.Swagger
{
    public class ApiExplorerApiControllersConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.ControllerType.Namespace;
            // Include only those controllers, whose namespace ends with .Api
            controller.ApiExplorer.IsVisible = controllerNamespace.EndsWith(".Api", StringComparison.OrdinalIgnoreCase);
        }
    }
}
