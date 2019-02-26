using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VirtoCommerce.Storefront.Infrastructure.Swagger
{
    public class OptionalParametersFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters?.Any() == true)
            {
                var optionalParameters = context.ApiDescription.ParameterDescriptions
                    .Where(x => x.ParameterDescriptor != null &&
                        ((ControllerParameterDescriptor)x.ParameterDescriptor).ParameterInfo.CustomAttributes.Any(attr => attr.AttributeType == typeof(SwaggerOptionalAttribute)))
                    .ToList();

                foreach (var apiParameter in optionalParameters)
                {
                    var parameter = operation.Parameters.FirstOrDefault(x => x.Name == apiParameter.Name);
                    if (parameter != null)
                    {
                        parameter.Required = false;
                    }
                }
            }
        }
    }
}
