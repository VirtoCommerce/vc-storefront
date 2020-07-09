using System.Linq;
using GraphQL;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class GraphQLResponseExtensions
    {
        public static GraphQLResponse<T> ThrowExceptionOnError<T>(this GraphQLResponse<T> response)
        {
            if (response.Errors != null && response.Errors.Any())
            {
                throw new StorefrontException(string.Join("\r\n", response.Errors.Select(e => e.Message)));
            }
            return response;
        }
    }
}
