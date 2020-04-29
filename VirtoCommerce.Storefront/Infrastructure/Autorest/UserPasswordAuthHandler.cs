using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace VirtoCommerce.Storefront.Infrastructure.Autorest
{
    /// <summary>
    /// HTTP message delegating handler that encapsulates access token handling and renewment
    /// </summary>
    public class UserPasswordAuthHandler : DelegatingHandler
    {
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private static string _accessToken;
        private bool _disposed;
        private readonly PlatformEndpointOptions _options;
        private readonly IHttpClientFactory _clientFactory;

        /// <summary>
        /// Gets or sets the timeout
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Gets the current access token
        /// </summary>
        public string AccessToken
        {
            get
            {
                if (_lock.Wait(Timeout))
                {
                    try
                    {
                        return _accessToken;
                    }
                    finally
                    {
                        _lock.Release();
                    }
                }

                return null;
            }
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="UserPasswordAuthHandler"/> class.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="clientFactory"></param>
        public UserPasswordAuthHandler(IOptions<PlatformEndpointOptions> options, IHttpClientFactory clientFactory)
        {
            _options = options.Value;
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

            var accessToken = await GetAccessTokenAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(accessToken) && (!(await RenewTokensAsync(cancellationToken))))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = request };
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            if (!(await RenewTokensAsync(cancellationToken)))
            {
                return response;
            }

            response.Dispose(); // This 401 response will not be used for anything so is disposed to unblock the socket.

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private async Task<bool> RenewTokensAsync(CancellationToken cancellationToken)
        {
            if (await _lock.WaitAsync(Timeout, cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    var client = _clientFactory.CreateClient();
                    var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest { Address = $"{_options.Url?.ToString().TrimEnd('/')}/connect/token", UserName = _options.UserName, Password = _options.Password });

                    if (!response.IsError)
                    {
                        _accessToken = response.AccessToken;
                        return true;
                    }
                }
                finally
                {
                    _lock.Release();
                }
            }

            return false;
        }

        private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (await _lock.WaitAsync(Timeout, cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    return _accessToken;
                }
                finally
                {
                    _lock.Release();
                }
            }

            return null;
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
            }

            base.Dispose(disposing);
        }
    }
}
