using System;

namespace VirtoCommerce.Storefront.Infrastructure.Swagger
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SwaggerSchemaIdAttribute : Attribute
    {
        public string Id { get; private set; }

        public SwaggerSchemaIdAttribute(string id)
        {
            Id = id;
        }
    }
}
