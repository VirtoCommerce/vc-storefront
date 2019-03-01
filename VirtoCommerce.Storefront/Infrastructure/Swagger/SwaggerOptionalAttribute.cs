using System;

namespace VirtoCommerce.Storefront.Infrastructure.Swagger
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public class SwaggerOptionalAttribute : Attribute
    {
    }
}
