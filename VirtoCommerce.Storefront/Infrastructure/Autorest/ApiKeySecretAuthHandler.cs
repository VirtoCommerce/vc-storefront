using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Infrastructure.Autorest
{
    public class ApiKeySecretAuthHandler : BaseAuthHandler
    {
        private readonly PlatformEndpointOptions _options;

        public ApiKeySecretAuthHandler(IOptions<PlatformEndpointOptions> options, IWorkContextAccessor workContextAccessor, IHttpContextAccessor httpContextAccessor) : base(workContextAccessor, httpContextAccessor)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
        /// </returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddAuthorization(request);
            AddCurrentUser(request);
            AddUserIp(request);

            return await base.SendAsync(request, cancellationToken);
        }

        private void AddAuthorization(HttpRequestMessage request)
        {
            if (_options != null)
            {
                var signature = new ApiRequestSignature { AppId = _options.AppId };

                var parameters = new[]
                {
                    new NameValuePair(null, _options.AppId),
                    new NameValuePair(null, signature.TimestampString)
                };

                signature.Hash = HmacUtility.GetHashString(key => new HMACSHA256(key), _options.SecretKey, parameters);
                request.Headers.Authorization = new AuthenticationHeaderValue("HMACSHA256", signature.ToString());
            }
        }
    }
}
