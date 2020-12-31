using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common.Caching;

namespace VirtoCommerce.Storefront.Infrastructure.Autorest
{
    /// <summary>
    /// HTTP message delegating handler that encapsulates access token handling and renewment
    /// Implements user-password authorization to the Platform API 
    /// </summary>
    public class UserPasswordAuthHandler : BaseAuthHandler
    {
        private readonly PlatformEndpointOptions _options;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IStorefrontMemoryCache _memoryCache;

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

              /// <summary>
        ///  Initializes a new instance of the <see cref="UserPasswordAuthHandler"/> class.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="clientFactory"></param>
        public UserPasswordAuthHandler(
            IStorefrontMemoryCache memoryCache
            , IOptions<PlatformEndpointOptions> options
            , IHttpClientFactory clientFactory
            , IWorkContextAccessor workContextAccessor
            , IHttpContextAccessor httpContextAccessor) : base(workContextAccessor, httpContextAccessor)
        {
            _options = options.Value;
            _clientFactory = clientFactory;
            _memoryCache = memoryCache;
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
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            return await base.SendAsync(request, cancellationToken);
        }

        protected override async Task AddAuthenticationAsync(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetOrRenewTokenAsync());
        }

        private async Task<string> GetOrRenewTokenAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "token");
            var token = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                var client = _clientFactory.CreateClient();

                var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest { Address = $"{_options.Url?.ToString().TrimEnd('/')}/connect/token", UserName = _options.UserName, Password = _options.Password });
                if (!response.IsError)
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(response.ExpiresIn);
                    return response.AccessToken;
                }
                return null;
            }, cacheNullValue: false);
            return token;
        }

  
    }
}
