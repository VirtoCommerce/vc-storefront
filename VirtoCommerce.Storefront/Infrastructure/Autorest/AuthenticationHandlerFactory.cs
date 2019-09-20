using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace VirtoCommerce.Storefront.Infrastructure.Autorest
{
    public class AuthenticationHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<PlatformEndpointOptions> _platformEndpointOptions;

        public AuthenticationHandlerFactory(IServiceProvider serviceProvider, IOptions<PlatformEndpointOptions> platformEndpointOptions)
        {
            _serviceProvider = serviceProvider;
            _platformEndpointOptions = platformEndpointOptions;
        }

        public DelegatingHandler CreateAuthHandler()
        {
            DelegatingHandler result = null;
            var options = _platformEndpointOptions.Value;

            if (!string.IsNullOrEmpty(options.AppId) && !string.IsNullOrEmpty(options.SecretKey))
            {
                result = _serviceProvider.GetService<ApiKeySecretAuthHandler>();
            }
            if (!string.IsNullOrEmpty(options.UserName) && !string.IsNullOrEmpty(options.Password))
            {
                result = _serviceProvider.GetService<UserPasswordAuthHandler>();
            }

            return result;
        }
    }
}
