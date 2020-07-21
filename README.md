# [Virto Commerce Storefront Kit](https://virtocommerce.com) [![Share on Facebook](https://img.shields.io/badge/facebook--blue.svg?style=social&label=Share&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz48IURPQ1RZUEUgc3ZnIFBVQkxJQyAiLS8vVzNDLy9EVEQgU1ZHIDEuMS8vRU4iICJodHRwOi8vd3d3LnczLm9yZy9HcmFwaGljcy9TVkcvMS4xL0RURC9zdmcxMS5kdGQiPjxzdmcgdmVyc2lvbj0iMS4xIiBpZD0iTGF5ZXJfMSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4bWxuczp4bGluaz0iaHR0cDovL3d3dy53My5vcmcvMTk5OS94bGluayIgeD0iMHB4IiB5PSIwcHgiIHdpZHRoPSIyNjYuODkzcHgiIGhlaWdodD0iMjY2Ljg5NXB4IiB2aWV3Qm94PSIwIDAgMjY2Ljg5MyAyNjYuODk1IiBlbmFibGUtYmFja2dyb3VuZD0ibmV3IDAgMCAyNjYuODkzIDI2Ni44OTUiIHhtbDpzcGFjZT0icHJlc2VydmUiPjxwYXRoIGlkPSJCbHVlXzFfIiBmaWxsPSIjM0M1QTk5IiBkPSJNMjQ4LjA4MiwyNjIuMzA3YzcuODU0LDAsMTQuMjIzLTYuMzY5LDE0LjIyMy0xNC4yMjVWMTguODEyYzAtNy44NTctNi4zNjgtMTQuMjI0LTE0LjIyMy0xNC4yMjRIMTguODEyYy03Ljg1NywwLTE0LjIyNCw2LjM2Ny0xNC4yMjQsMTQuMjI0djIyOS4yN2MwLDcuODU1LDYuMzY2LDE0LjIyNSwxNC4yMjQsMTQuMjI1SDI0OC4wODJ6Ii8%2BPHBhdGggaWQ9ImYiIGZpbGw9IiNGRkZGRkYiIGQ9Ik0xODIuNDA5LDI2Mi4zMDd2LTk5LjgwM2gzMy40OTlsNS4wMTYtMzguODk1aC0zOC41MTVWOTguNzc3YzAtMTEuMjYxLDMuMTI3LTE4LjkzNSwxOS4yNzUtMTguOTM1bDIwLjU5Ni0wLjAwOVY0NS4wNDVjLTMuNTYyLTAuNDc0LTE1Ljc4OC0xLjUzMy0zMC4wMTItMS41MzNjLTI5LjY5NSwwLTUwLjAyNSwxOC4xMjYtNTAuMDI1LDUxLjQxM3YyOC42ODRoLTMzLjU4NXYzOC44OTVoMzMuNTg1djk5LjgwM0gxODIuNDA5eiIvPjwvc3ZnPg%3D%3D)](https://www.facebook.com/sharer.php?u=https://virtocommerce.com)&nbsp;[![Tweet](https://img.shields.io/twitter/url/https/virtocommerce.com.svg?style=social)](https://twitter.com/intent/tweet?text=%23VirtoCommerce%20puts%20the%20best%20of%20MS%20Azure%20Cloud%2C%20open%20source%20.Net%20code%20and%20agile%20development%20in%20a%20single%20enterprise%20%23ecommerce%20platform.) [![Latest release](https://img.shields.io/github/release/VirtoCommerce/vc-storefront.svg)](https://github.com/VirtoCommerce/vc-storefront/releases/latest) [![Total downloads](https://img.shields.io/github/downloads/VirtoCommerce/vc-storefront/total.svg?colorB=007ec6)](https://github.com/VirtoCommerce/vc-storefront/releases) [![License](https://img.shields.io/badge/license-VC%20OSL-blue.svg)](https://virtocommerce.com/open-source-license)

> Precaution! Storefront version 5 and above is backward NON-compatible with VirtoCommerce platform v2! Use only with platform v3.

[![Build status](http://ci.virtocommerce.com/buildStatus/icon?job=vc-2-org/vc-storefront/master)](http://ci.virtocommerce.com/job/vc-2-org/job/vc-storefront/job/master/) [![Quality Gate](https://sonar.virtocommerce.com/api/badges/gate?key=vc-storefront%3Amaster&blinking=true)](https://sonar.virtocommerce.com/dashboard?id=vc-storefront%3Amaster) [![Lines of code](https://sonar.virtocommerce.com/api/badges/measure?key=vc-storefront%3Amaster&metric=ncloc)](https://sonar.virtocommerce.com/api/badges/measure?key=vc-storefront%3Amaster&metric=ncloc)&emsp;
[![Documentation](https://img.shields.io/badge/docs-read-brightgreen.svg)](https://virtocommerce.com/docs/latest/)&nbsp;[![Discourse topics](https://img.shields.io/discourse/topics?label=community&logo=community&server=http%3A%2F%2Fwww.virtocommerce.org)](https://www.virtocommerce.org) [![Contributors](https://img.shields.io/github/contributors/VirtoCommerce/vc-storefront.svg)](https://github.com/VirtoCommerce/vc-storefront/graphs/contributors)

[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

Official online shopping website based on Virto Commerce Platform written on ASP.NET Core. The website is a client application for VC Platform and uses only public APIs while communicating.

It is possible to run multiple different stores (web sites) on top of Virto Commerce. Each store (website) might have its own theme with a specific layout still being based on the same catalog and customer data.

It is possible, for example, to create sites with a different design for different product categories still having all products in the same backend.

Another option is to create different sites for different regions so that a specific product set will be available to a specific region, is still based on one product data.

It is also possible to connect Virto Commerce to multiple touchpoints so that customers will get a real omnichannel experience.

## Key features
* Multi-Store support
* Multi-Language support
* Multi-Currency support
* Multi-Themes support
* Faceted search support
* SEO friendly routing
* Server-side rendering
* Client-side rendering
* Optimization for Desktop
* Optimization for Tables
* Optimization for Mobile

## Business Requirements
* Home page
* Catalog browsing
* Product page
* Cart
* Bulk add to cart
* Multiple Whish lists and Mark favorites
* Product compare
* Anonymous Checkout process
* Checkout process
* Address verification
* Integration with Tax providers
* Integration Shippment and Payment methods
* Place orders
* Order Approve process
* Offers
* My Account
* My Orders History
* Reorder
* Reorder
* New Account verification
* Self-registration
* Forgot password
* User permissions to manage their own account page
* Catalog personalization
* Price personalization (List and Sales prices)
* Promotions
* Coupons
* Recommended products
* Banner and dynamic content
* User Groups - to build personalization
* Configurable Branding and customization
* Configurable navigation
* CMS to build Static, Landing, Blog pages

## References
* [Public Demo](https://demo.virtocommerce.com)
* Virto Commerce Documentation: https://www.virtocommerce.com/docs/latest/
* Home: https://virtocommerce.com
* Community: https://www.virtocommerce.org
* [Download Latest Release](https://github.com/VirtoCommerce/vc-storefront/releases/latest)


## Sample themes

### [Default theme](https://github.com/VirtoCommerce/vc-theme-default)
![electronics](https://user-images.githubusercontent.com/7566324/31821605-f36d17de-b5a5-11e7-9bb5-a71803285d8b.png)

### [B2B theme](https://github.com/VirtoCommerce/vc-theme-b2b)
![img_20102017_174148_0](https://user-images.githubusercontent.com/7566324/31821606-f3974b26-b5a5-11e7-8b52-e3b80d6bdd74.png)

### [Procurement Portal](https://github.com/VirtoCommerce/vc-procurement-portal-theme)
![img_20102017_174148_0](https://github.com/VirtoCommerce/vc-procurement-portal-theme/raw/master/docs/media/screen-catalog-page.png)

### [Business Portal](https://github.com/VirtoCommerce/vc-odt-mpa-theme)
![img_20102017_174148_0](https://user-images.githubusercontent.com/7566324/80617364-4ba80a80-8a42-11ea-8222-be17dea1cc8d.png)


## Technologies and frameworks used
* ASP.NET MVC Core 3.1.0 on .NET Core 3.1.0
* ASP.NET Identity Core 3.1.0
* REST services clients generation with using [Microsoft AutoRest](https://github.com/Azure/autorest)
* Liquid view engine based on [Scriban](https://github.com/lunet-io/scriban)
* [LibSassHost](https://github.com/Taritsyn/LibSassHost) for processing **scss** stylesheets in runtime


## List of changes
1. Changed settings, now we are using a [new approach](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration) recommended by ASP.NET Core, we are using the appsettings.json file and strongly type options for working with settings from code.
2. Authentication and authorization was completely rewritten according to using [ASP.NET Identity Core](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity).
3. Default ASP.NET Core [in-memory caching](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory) completely replaced the [CacheManager](http://cachemanager.michaco.net/) used before.
4. New more selective cache invalidation based on usage of `CancellationChangeToken` and strongly typed cache regions allows to display always actual content without performance lossing.
5. New framework for working with domain events.
6. Usage of [ASP.NET Core middlewares](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware)
7. Reworked the WorkContext initialization, it made more fluently.
8. Usage of the latest version of [Microsoft AutoRest](https://github.com/Azure/autorest)
9. Usage  of [ASP.NET Core Response Caching Middleware](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware) for FPC (full page caching).
10. Use [Build-in ASP.NET Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) instead Unity DI and IoC container.


## Continuous Integration Supported by Browserstack
Cross-browser compatibility of the storefront is generously provided by [Browserstack](https://www.browserstack.com/).

[![Browserstack](https://images.techhive.com/images/article/2014/12/browserstack-logo-100538202-medium.idge.png)](http://browserstack.com/)


## Deploy Storefront

> If Platform and Storefront are deployed in the same on-premises environment, Storefront should be deployed on different port then Platform. You can do it by `dotnet run CLI`

### Downloading the precomplied binaries

* Navigate to the [Releases section of Virto Commerce Storefront Kit in GitHub](https://github.com/VirtoCommerce/vc-storefront/releases).

* You will find **VirtoCommerce.Storefront.5.x.x.zip** file. In this file the site has already been built and can be run without additional compilation. The source code is not included.

* Unpack this zip to a local directory **C:\vc-storefront**. After that you will have the directory with Storefront precompiled files.

### Setup

#### Configure application strings

* Open the **appsettings.json** file in a text editor.
* In the **Endpoint** section change **Url**, **UserName**, **Password** with correct path and credentials for Virto Commerce Platform:

```json
...
 "Endpoint": {
     "Url": "https://localhost:5001",
     "UserName": "admin",
     "Password": "store",
```

#### Configure CMS Content storage

Storefront  **appsettings.json** file contains **ContentConnectionString** setting with pointed to the folder with actual themes and pages content
```json
...
"ConnectionStrings": {
    //For themes stored in local file system
    "ContentConnectionString": "provider=LocalStorage;rootPath=~/cms-content"
	//For themes stored in azure blob storage
    //"ContentConnectionString" connectionString="provider=AzureBlobStorage;rootPath=cms-content;DefaultEndpointsProtocol=https;AccountName=yourAccountName;AccountKey=yourAccountKey"
  },
...
```

You can set this connection string in one of the following ways:

1. Build and Copy theme to `wwwroot\cms-content\{StoreName}\{ThemeName}`
1. If you have already have installed  platform with sample data, your platform already contains `~/App_Data/cms-content` folder with themes for sample stores and you need only to make symbolic link to this folder by this command:
    ```console
    mklink /d C:\vc-storefront\VirtoCommerce.Storefront\wwwroot\cms-content C:\vc-platform\VirtoCommerce.Platform.Web\App_Data\cms-content
    ```
On Mac OS and Linux:
    ```console
    ln -s ~/vc-storefront/wwwroot/cms-content ~/vc-platform/wwwroot/cms-content
    ```
1. If you did not install sample data with your platform, you need to create new store in platform manager and download themes as it described in this article: [Theme development](../fundamentals/theme-development.md)

#### Running the Storefront only on HTTP schema
 
* In order to run the platform only at HTTP schema in production mode, it's enough to pass only HTTP URLs in `--urls` argument of the `dotnet` command.

```console
  dotnet VirtoCommerce.Storefront.dll --urls=http://localhost:5002
```

#### Running the Platform on HTTPS schema

* Install and trust HTTPS certificate

Run to trust the .NET Core SDK HTTPS development certificate:

```console
    dotnet dev-certs https --trust
```

Read more about [enforcing HTTPS in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-3.0&tabs=visual-studio#trust)


```console
    dotnet VirtoCommerce.Storefront.dll --urls=https://localhost:4302/
```


* Trust the .Net Core Development Self-Signed Certificate. More details on trusting the self-signed certificate can be found [here](https://blogs.msdn.microsoft.com/robert_mcmurray/2013/11/15/how-to-trust-the-iis-express-self-signed-certificate/)



## License
Copyright (c) Virto Solutions LTD.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
