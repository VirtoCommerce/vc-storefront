using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VirtoCommerce.Storefront.Infrastructure.Swagger
{
    public class FileResponseTypeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (IsFileResponse(context.ApiDescription))
            {
                var responseSchema = new OpenApiSchema { Format = "byte", Type = "file" };

                var okStatusString = ((int)HttpStatusCode.OK).ToString();
                var okStatusResponse = new OpenApiResponse
                {
                    Description = "OK",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = responseSchema
                        }
                    }
                };
                if (operation.Responses.Any(x => x.Key == okStatusString))
                {
                    operation.Responses[okStatusString] = okStatusResponse;
                }
                else
                {
                    operation.Responses.Add(okStatusString, okStatusResponse);
                }
            }
        }

        private static bool IsFileResponse(ApiDescription apiDescription)
        {
            apiDescription.TryGetMethodInfo(out var methodInfo);
            var result = methodInfo.GetCustomAttributes<SwaggerFileResponseAttribute>().Any();
            if (!result)
            {
                result = apiDescription.SupportedResponseTypes.Any(r => r.Type == typeof(Stream));
            }
            return result;
        }
    }
}
