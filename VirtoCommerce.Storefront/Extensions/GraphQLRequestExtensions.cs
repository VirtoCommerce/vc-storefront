using System.Collections.Generic;
using GraphQL;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class GraphQLRequestExtensions
    {
        public static GraphQLRequest CreateMutation(string name, IDictionary<object, object> variables, params KeyValuePair<string, string>[] parameters)
        {
            //var query = $"mutation ({string.Join(',', parameters.Select(p => $"${p.Key}:{p.Value}"))})";
            //var mutation = new GraphQLRequest
            //{

            //};
        }
    }
}
