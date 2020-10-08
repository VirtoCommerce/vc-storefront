using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Middleware
{
    public class WorkContextBuildMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly StorefrontOptions _options;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly Dictionary<string, object> _applicationSettings;
        public WorkContextBuildMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnvironment,
                                          IOptions<StorefrontOptions> options, IWorkContextAccessor workContextAccessor, IConfiguration configuration)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
            _workContextAccessor = workContextAccessor;
            _options = options.Value;
            _configuration = configuration;

        }

        public async Task Invoke(HttpContext context)
        {
            //Do not process for exist exception 
            var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature != null)
            {
                await _next(context);
                return;
            }

            var builder = new WorkContextBuilder(context, _options);
            var workContext = builder.WorkContext;

            workContext.IsDevelopment = _hostingEnvironment.IsDevelopment();
            workContext.ApplicationSettings = _applicationSettings;
            //The important to preserve the order of initialization
            await builder.WithCountriesAsync();

            await builder.WithStoresAsync(_options.DefaultStore);
            await builder.WithCurrentUserAsync();
            await builder.WithCurrenciesAsync(workContext.CurrentLanguage, workContext.CurrentStore);

            await builder.WithMenuLinksAsync(workContext.CurrentStore, workContext.CurrentLanguage);
            await builder.WithPagesAsync(workContext.CurrentStore, workContext.CurrentLanguage);
            await builder.WithBlogsAsync(workContext.CurrentStore, workContext.CurrentLanguage);
       

            //EU General Data Protection Regulation (GDPR) support 
            var consentFeature = context.Features.Get<ITrackingConsentFeature>();
            if (consentFeature != null)
            {
                workContext.CanTrack = !consentFeature?.CanTrack ?? false;
                workContext.ConsentCookie = consentFeature?.CreateConsentCookie();
            }
            workContext.AvailableRoles = SecurityConstants.Roles.AllRoles;
            workContext.BusinessToBusinessRoles = SecurityConstants.Roles.B2BRoles;
            _workContextAccessor.WorkContext = workContext;


            await _next(context);
        }
    }

}
