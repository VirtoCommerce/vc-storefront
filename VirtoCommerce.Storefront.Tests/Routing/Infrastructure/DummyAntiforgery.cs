using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace VirtoCommerce.Storefront.Tests.Routing.Infrastructure
{
    /// <summary>
    /// Test implementation of <see cref="IAntiforgery"/> that completely disables antiforgery checks.
    /// </summary>
    public class DummyAntiforgery : IAntiforgery
    {
        private static readonly AntiforgeryTokenSet EmptyAntiforgeryTokenSet =
            new AntiforgeryTokenSet(string.Empty, string.Empty, string.Empty, string.Empty);


        /// <inheritdoc />
        public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
        {
            return EmptyAntiforgeryTokenSet;
        }

        /// <inheritdoc />
        public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
        {
            return EmptyAntiforgeryTokenSet;
        }

        /// <inheritdoc />
        public Task<bool> IsRequestValidAsync(HttpContext httpContext)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task ValidateRequestAsync(HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void SetCookieTokenAndHeader(HttpContext httpContext)
        {
            // This IAntiforgery implementation does not perform any checks,
            // so body of this method intentionally left blank.
        }
    }
}
