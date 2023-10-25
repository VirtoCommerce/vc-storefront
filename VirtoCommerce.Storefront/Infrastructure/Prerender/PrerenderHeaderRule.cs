using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Rewrite;

namespace VirtoCommerce.Storefront.Infrastructure.Prerender;

public class PrerenderHeaderRule: IRule
{
    private string _token;

    public PrerenderHeaderRule(string token)
    {
        _token = token;
    }

    public void ApplyRule(RewriteContext context)
    {
        var request = context.HttpContext.Request;

        if (request.Host.HasValue && request.Host.Host.Contains("prerender.io"))
        {
            request.Headers.TryAdd("X-Prerender-Token", _token);
        }
    }
}
