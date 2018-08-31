using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace VirtoCommerce.Storefront.Tests.Routing.Infrastructure
{
    public class DummyAntiforgery : IAntiforgery
    {
        private static readonly AntiforgeryTokenSet EmptyAntiforgeryTokenSet =
            new AntiforgeryTokenSet(string.Empty, string.Empty, string.Empty, string.Empty);


        public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
        {
            return EmptyAntiforgeryTokenSet;
        }

        public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
        {
            return EmptyAntiforgeryTokenSet;
        }

        public Task<bool> IsRequestValidAsync(HttpContext httpContext)
        {
            return Task.FromResult(true);
        }

        public Task ValidateRequestAsync(HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        public void SetCookieTokenAndHeader(HttpContext httpContext)
        {
            // This IAntiforgery implementation does not perform any checks,
            // so body of this method intentionally left blank.
        }
    }
}
