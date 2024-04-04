# Virto Commerce Storefront Kit

[![CI status](https://github.com/VirtoCommerce/vc-storefront/workflows/Storefront%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-storefront/actions?query=workflow%3A"Storefront+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-storefront&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-storefront) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-storefront&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-storefront) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-storefront&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-storefront) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-storefront&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-storefront) [![Lines of code](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-storefront&branch=dev&metric=ncloc)](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-storefront&branch=dev&metric=ncloc)&emsp;

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FVirtoCommerce%2Fvc-storefront%2Fmaster%2Fazuredeploy.json)

The Virto Commerce Storefront Kit is the official online shopping website based on the Virto Commerce Platform, written on ASP.NET 8. The website serves as a client application for the VC Platform and communicates solely through public APIs.

The Storefront Kit enables the creation of multiple distinct stores (websites) on top of the Virto Commerce Platform. Each store may have its own theme with specific layouts, yet still be based on the same catalog and customer data. This allows for versatile store configurations, such as:

* Different designs for various product categories.
* Regional-specific sites offering tailored product sets.
* Integration with multiple touchpoints for a true omnichannel experience.


## Key features
- Launch and host e-commerce themes on top of the Virto Commerce Platform.
- XAPI Gateway.
- Caching mechanism.
- Multi-store support.
- Multi-theme support.
- Server-side rendering.
And more.

## Architecture
For detailed information about the Virto Storefront Architecture, please refer to our [developer guide](https://docs.virtocommerce.org/storefront/developer-guide/)

## Technologies and frameworks used
- ASP.NET 8
- ASP.NET Identity Core

## Setup
For detailed setup information, please refer to [Quick Start](https://docs.virtocommerce.org/storefront/developer-guide/getting-started/quickstart-on-windows/) to deploy and run.


## Themes

### B2B Theme

View [B2B theme on GitHub](https://github.com/VirtoCommerce/vc-theme-b2b-vue).

![image](https://user-images.githubusercontent.com/7639413/170992875-fbfa2093-ebbf-4404-8140-c952d9f0f0f4.png)


### FAQ

#### Running the Storefront only on HTTP schema

- In order to run the platform only at HTTP schema in production mode, it's enough to pass only HTTP URLs in `--urls` argument of the `dotnet` command.

```console
  dotnet VirtoCommerce.Storefront.dll --urls=http://localhost:5002
```

#### Running the Platform on HTTPS schema

- Install and trust HTTPS certificate

Run to trust the .NET Core SDK HTTPS development certificate:

```console
    dotnet dev-certs https --trust
```

Read more about [enforcing HTTPS in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-3.0&tabs=visual-studio#trust)

```console
    dotnet VirtoCommerce.Storefront.dll --urls=https://localhost:4302/
```

- Trust the .Net Core Development Self-Signed Certificate. More details on trusting the self-signed certificate can be found [here](https://blogs.msdn.microsoft.com/robert_mcmurray/2013/11/15/how-to-trust-the-iis-express-self-signed-certificate/)

#### Forward the scheme for Linux and non-IIS reverse proxies

Apps that call UseHttpsRedirection and UseHsts put a site into an infinite loop if deployed to an Azure Linux App Service, Azure Linux virtual machine (VM), Linux container or behind any other reverse proxy besides IIS. TLS is terminated by the reverse proxy, and Kestrel isn't made aware of the correct request scheme. OAuth and OIDC also fail in this configuration because they generate incorrect redirects. UseIISIntegration adds and configures Forwarded Headers Middleware when running behind IIS, but there's no matching automatic configuration for Linux (Apache or Nginx integration).

To forward the scheme from the proxy in non-IIS scenarios, set `ASPNETCORE_FORWARDEDHEADERS_ENABLED` environment variable to `true`.

For more details on how it works, see the Microsoft [documentation](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-5.0#forward-the-scheme-for-linux-and-non-iis-reverse-proxies).

## References
- Virto Commerce Documentation: https://docs.virtocommerce.org
- Home: https://virtocommerce.com
- Community: https://www.virtocommerce.org
- [Download Latest Release](https://github.com/VirtoCommerce/vc-storefront/releases/latest)


## License

Copyright (c) Virto Solutions LTD. All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

<http://virtocommerce.com/opensourcelicense>

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
