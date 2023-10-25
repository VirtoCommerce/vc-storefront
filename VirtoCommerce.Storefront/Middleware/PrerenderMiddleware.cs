

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Infrastructure.Prerender;

namespace VirtoCommerce.Storefront.Middleware;

public class PrerenderMiddleware
{
    private readonly RequestDelegate _next;
    private readonly PrerenderOptions _options;

    public PrerenderMiddleware(RequestDelegate next, IOptions<PrerenderOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;

        if (request.Host.HasValue && request.Host.Host.Contains("prerender.io"))
        {
            request.Headers.TryAdd("HTTP_X_PRERENDER_TOKEN", _options.Token);
        }

        await _next(context);
    }
}
