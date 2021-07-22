namespace VirtoCommerce.Storefront.Infrastructure
{
    public class GraphQLExecutionResult
    {
        public GraphQLError[] Errors { get; set; }
    }

    public class GraphQLError
    {
        public GraphQLExtensions Extensions { get; set; }
    }

    public class GraphQLExtensions
    {
        public string Code { get; set; }
    }
}
