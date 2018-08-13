using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CustomCookieAuthenticationHandler : CookieAuthenticationHandler
    {
        public CustomCookieAuthenticationHandler(IOptionsMonitor<CookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            await HandleExpirationAsync();
            return await base.HandleAuthenticateAsync();
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            var handleExpirationResult = await HandleExpirationAsync();
            if (handleExpirationResult.Succeeded)
            {
                await RedirectToLoginAsync(handleExpirationResult.Ticket.Properties);
            }
            else
            {
                await base.HandleForbiddenAsync(properties);
            }
        }

        protected virtual async Task<AuthenticateResult> HandleExpirationAsync()
        {
            var authenticationTicket = GetAuthenticationTicket();
            if (authenticationTicket?.Properties != null && IsCookieExpired(authenticationTicket.Properties) && Options.LoginPath.HasValue)
            {
                await RedirectToLoginAsync(authenticationTicket.Properties);
                return AuthenticateResult.Success(authenticationTicket);
            }
            return AuthenticateResult.Fail("Cookie doesn't exist or doesn't expired");
        }

        private AuthenticationTicket GetAuthenticationTicket()
        {
            var cookie = Options.CookieManager.GetRequestCookie(Context, Options.Cookie.Name);
            if (!string.IsNullOrEmpty(cookie))
            {
                return Options.TicketDataFormat.Unprotect(cookie, GetTlsTokenBinding());
            }
            return null;
        }

        private string GetTlsTokenBinding()
        {
            var binding = Context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }

        private async Task RedirectToLoginAsync(AuthenticationProperties properties)
        {
            var redirectUri = properties.RedirectUri;
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = OriginalPathBase + Request.Path + Request.QueryString;
            }
            var loginUri = Options.LoginPath + QueryString.Create(Options.ReturnUrlParameter, redirectUri);
            await Events.RedirectToLogin(new RedirectContext<CookieAuthenticationOptions>(Context, Scheme, Options, properties, BuildRedirectUri(loginUri)));
        }

        private bool IsCookieExpired(AuthenticationProperties properties)
        {
            return properties.ExpiresUtc != null && properties.ExpiresUtc.Value < Clock.UtcNow;
        }
    }
}
