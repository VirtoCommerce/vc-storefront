using System;

namespace VirtoCommerce.Storefront.Infrastructure.Swagger
{
    public class ApiSchemaOptions
    {
        public bool NameAndOrderRequestBody { get; set; }

        public bool NotNullableReferenceTypesInArrays { get; set; }

        public OpenApiSpecificationVersion OpenApiSpecificationVersion { get; set; }
    }
}
