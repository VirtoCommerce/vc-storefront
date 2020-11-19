using System.Net.Http;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Infrastructure.Autorest
{
    public class BaseAuthHandler : DelegatingHandler
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseAuthHandler(IWorkContextAccessor workContextAccessor, IHttpContextAccessor httpContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
            _httpContextAccessor = httpContextAccessor;
        }

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

        protected void AddUserIp(HttpRequestMessage request)
        {
            var userIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            if (!string.IsNullOrEmpty(userIp))
            {
                request.Headers.Add("True-Client-IP", userIp);
            }
        }
    }
}
