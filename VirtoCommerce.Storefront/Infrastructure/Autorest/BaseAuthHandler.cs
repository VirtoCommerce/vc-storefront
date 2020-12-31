using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Infrastructure.Autorest
{
    /// <summary>
    /// Basic implementation of authorization handlers to the Platform API 
    /// </summary>
    public abstract class BaseAuthHandler : DelegatingHandler
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseAuthHandler(IWorkContextAccessor workContextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Add platform-specific headers and authentification, then call basic SendAsync
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddCurrentUser(request);
            AddUserIp(request);
            await AddAuthenticationAsync(request);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Add end-user name to the reqiest header
        /// </summary>
        /// <param name="request"></param>
        protected void AddCurrentUser(HttpRequestMessage request)
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
        /// Add end-user IP to the reqiest header
        /// </summary>
        /// <param name="request"></param>
        protected void AddUserIp(HttpRequestMessage request)
        {
            var userIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            if (!string.IsNullOrEmpty(userIp))
            {
                request.Headers.Add("True-Client-IP", userIp);
            }
        }

        /// <summary>
        /// Add authentication details to the request.
        /// This method should be implemented depending on authorization way
        /// </summary>
        /// <param name="request"></param>
        protected abstract Task AddAuthenticationAsync(HttpRequestMessage request);
    }
}
