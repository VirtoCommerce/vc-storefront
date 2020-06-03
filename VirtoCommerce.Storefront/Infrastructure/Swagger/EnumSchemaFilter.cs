using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VirtoCommerce.Storefront.Infrastructure.Swagger
{
    /// <summary>
    /// Compatibility: publish enumeration values as strings instead of integers (default in openapi 3)
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (type.IsEnum)
            {
                schema.Format = null;
                schema.Type = "string";
                schema.Enum.Clear();
                foreach (var enumValueInString in Enum.GetNames(type))
                {
                    schema.Enum.Add(new OpenApiString(enumValueInString));
                }
            }
        }
    }
}
