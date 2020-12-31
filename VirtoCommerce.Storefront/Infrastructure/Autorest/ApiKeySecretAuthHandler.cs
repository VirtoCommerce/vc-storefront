using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Infrastructure.Autorest
{
    /// <summary>
    /// HTTP message delegating handler that encapsulates access token handling and renewment
    /// Implements key-secret authorization to the Platform API 
    /// </summary>
    public class ApiKeySecretAuthHandler : BaseAuthHandler
    {
        private readonly PlatformEndpointOptions _options;

        public ApiKeySecretAuthHandler(IOptions<PlatformEndpointOptions> options, IWorkContextAccessor workContextAccessor, IHttpContextAccessor httpContextAccessor) : base(workContextAccessor, httpContextAccessor)
        {
            _options = options.Value;
        }

        protected override Task AddAuthenticationAsync(HttpRequestMessage request)
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
            return Task.CompletedTask;
        }
    }
}
