using System;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VirtoCommerce.Storefront.Infrastructure.Swagger
{
    public class EnumDefaultValueParameterFilter : IParameterFilter
    {
        public void Apply(IParameter parameter, ParameterFilterContext context)
        {
            var parameterType = context.ApiParameterDescription.Type;
            var paramDefaultValue = context.ApiParameterDescription.DefaultValue;
            if (paramDefaultValue != null && parameterType.IsEnum)
            {
                var partialSchema = parameter as PartialSchema;
                partialSchema.Default = Enum.GetName(parameterType, paramDefaultValue);
            }
        }
    }
}
