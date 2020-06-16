using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public class StorefrontApplicationFactory : WebApplicationFactory<Storefront.Startup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(x =>
                {
                    x.UseStartup<Storefront.Startup>().UseTestServer();
                });
            return builder;
        }
    }
}
