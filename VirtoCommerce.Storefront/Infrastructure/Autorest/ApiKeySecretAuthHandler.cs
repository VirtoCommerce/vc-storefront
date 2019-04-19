using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Infrastructure.Autorest
{
    public class ApiKeySecretAuthHandler : DelegatingHandler
    {
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private bool _disposed;
        /// <summary>
        /// Instance of used HttpClient
        /// </summary>
        private static HttpClient _client;

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly PlatformEndpointOptions _options;
        private readonly IHttpClientFactory _clientFactory;

        public ApiKeySecretAuthHandler(IOptions<PlatformEndpointOptions> options, IWorkContextAccessor workContextAccessor, IHttpClientFactory clientFactory)
        {
            _options = options.Value;
            _workContextAccessor = workContextAccessor;
            _clientFactory = clientFactory;
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
            if (_client == null)
            {
                CreateHttpClient();
            }

            AddAuthorization(request);
            AddCurrentUser(request);

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

        private void AddCurrentUser(HttpRequestMessage request)
        {
            var worContex = _workContextAccessor.WorkContext;
            if (worContex != null)
            {
                var currentUser = worContex.CurrentUser;

                //Add special header with user name to each API request for future audit and logging
                if (currentUser != null && currentUser.IsRegisteredUser)
                {
                    var userName = currentUser.OperatorUserName;

                    if (string.IsNullOrEmpty(userName))
                    {
                        userName = currentUser.UserName;
                    }

                    if (!string.IsNullOrEmpty(userName))
                    {
                        request.Headers.Add("VirtoCommerce-User-Name", userName);
                    }
                }
            }
        }

        /// <summary>
        /// Creates one HttpClient instance. It will be used for all requests.
        /// It is necessary to avoid exception "This instance has already started one or more requests. Properties can only be modified before sending the first request."
        /// if every autorest client would try to create its own HttpClient
        /// </summary>
        private void CreateHttpClient()
        {
            if (_lock.Wait(Timeout))
            {
                try
                {
                    if (_client == null)
                    {
                        _client = _clientFactory.CreateClient();
                    }
                }
                finally
                {
                    _lock.Release();
                }
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Net.Http.DelegatingHandler" />, and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _lock.Dispose();
                _client.Dispose();
                _client = null;
            }

            base.Dispose(disposing);
        }
    }
}
