using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace VirtoCommerce.Storefront
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
              .UseApplicationInsights()
              .UseContentRoot(Directory.GetCurrentDirectory())
              .UseIISIntegration()
              .ConfigureLogging((hostingContext, logging) =>
              {
                  logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                  logging.AddConsole();
                  logging.AddDebug();
              })
              .UseStartup<Startup>();

    }

}
