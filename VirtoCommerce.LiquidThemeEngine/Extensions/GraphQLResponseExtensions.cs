using System;
using System.Collections.Generic;
using System.Text;
using GraphQL;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.LiquidThemeEngine.Extensions
{
    public static class GraphQLResponseExtensions
    {
        public static void ThrowIfHasErrors(this IGraphQLResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (!response.Errors.IsNullOrEmpty())
            {
                throw new StorefrontException(JsonConvert.SerializeObject(response.Errors, Formatting.Indented));
            }
        }
    }
}
