using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class HostingEnviromentExtension
    {
        public static string MapPath(this IHostingEnvironment hostEnv, string path)
        {
            var result = hostEnv.WebRootPath;

            if (path.StartsWith("~/"))
            {
                result = System.IO.Path.Combine(result, path.Replace("~/", string.Empty));
            }
            else if (Path.IsPathRooted(path))
            {
                result = path;
            }        

            return result;
        }
    }
}
