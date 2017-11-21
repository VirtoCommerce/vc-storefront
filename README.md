# VirtoCommerce Storefront for ASP.NET Core 2.0
VirtoCommerce Storefront represents the official online shopping website based on VirtoCommerce Platform written on ASP.NET Core. The website is a client application for VC Platform and uses only public APIs while communicating.

Technologies and frameworks used:
* ASP.NET MVC Core 2.0.0 on .NET Core 2.0.0
* ASP.NET Identity Core 2.0.0
* REST services clients generation with using [Microsoft AutoRest](https://github.com/Azure/autorest)
* Liquid view engine based on [DotLiquid](https://github.com/dotliquid/dotliquid)
* [LibSassHost](https://github.com/Taritsyn/LibSassHost) for processing **scss** stylesheets in runtime


Key features:
* Multi-Store support
* Multi-Language support
* Multi-Currency support
* Multi-Themes support
* Faceted search support
* SEO friendly routing

### List of changes
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

# Sample themes 

## [Default theme](https://github.com/VirtoCommerce/vc-theme-default)
![electronics](https://user-images.githubusercontent.com/7566324/31821605-f36d17de-b5a5-11e7-9bb5-a71803285d8b.png)

## [Material theme](https://github.com/VirtoCommerce/vc-theme-material)
![clothing](https://user-images.githubusercontent.com/7566324/31821604-f341c444-b5a5-11e7-877a-eb919e01dee2.PNG)

## [B2B theme](https://github.com/VirtoCommerce/vc-theme-b2b)
![img_20102017_174148_0](https://user-images.githubusercontent.com/7566324/31821606-f3974b26-b5a5-11e7-8b52-e3b80d6bdd74.png)

# Live DEMO
http://demo.virtocommerce.com

# Cloud Private Demo
Setup your own private Microsoft Cloud environment and evaluate the latest version of Virto Commerce Storefront, <a href="http://docs.virtocommerce.com/x/VwAqAQ" target="_blank">read more</a>.

<a href="https://azuredeploy.net/" target="_blank">
  <img alt="Deploy to Azure" src="http://azuredeploy.net/deploybutton.png"/>
</a>


# Documentation

## Source code getting started

### Prerequisites 
[Prerequisites for .NET Core on Windows](https://docs.microsoft.com/en-us/dotnet/core/windows-prerequisites)

### Downloading source code

Fork your own copy of VirtoCommerce Storefront to your account on GitHub:

1. Open VirtoCommerce Storefront in GitHub and click Fork in the upper right corner.
If you are a member of an organization on GitHub, select the target for the fork.
2. Clone the forked repository to local machine:
```
git clone https://github.com/<<your GitHub user name>>/vc-storefront.git C:\vc-storefront
```
3. Switch to the cloned directory:

```cd C:\vc-storefront```

4. Add a reference to the original repository:

```git remote add upstream https://github.com/VirtoCommerce/vc-storefront.git```

In result you should get the C:\vc-storefront folder which contains full storefront source code. To retrieve changes from original Virto Commerce Storefront repository, merge upstream/master branch.

### Configuring VirtoCommerce Platform Endpoint
Set actual platform endpoint values in the C:\vc-storefront\VirtoCommerce.Storefront\appsettings.json.
Read more about how to generate API keys [here](https://virtocommerce.com/docs/vc2devguide/development-scenarios/working-with-platform-api)

``` 
 ...
  "VirtoCommerce": {
    "Endpoint": {
	   //Virto Commerce platform manager url 
      "Url": "http://localhost/admin",
	   //HMAC authentification user credentials on whose behalf the API calls will be made.
      "AppId": "Enter your AppId here" 
      "SecretKey": "Enter your SecretKey here",
    }
	...
```
ASP.NET Core represents a new tools a **Secret Manager tool**, which allows in development to keep secrets out of your code. 
You can find more about them [here](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio)

### Configure themes 
Storefront  **appsettings.json** file contains **ContentConnectionString** setting with pointed to the folder with actual themes and pages content
```
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
1. If you have already have installed  platform with sample data, your platform already contains `~/App_Data/cms-content` folder with themes for sample stores and you need only to make symbolic link to this folder by this command:
```
mklink /d C:\vc-storefront\VirtoCommerce.Storefront\wwwroot\cms-content C:\vc-platform\VirtoCommerce.Platform.Web\App_Data\cms-content
```
2. If you did not install sample data with your platform, you need to create new store in platform manager and download themes as it described in this article 
[Theme development](https://virtocommerce.com/docs/vc2devguide/working-with-storefront/theme-development)

### Host on Windows with IIS 
VirtoCommerce.Storefront project already include the **web.config** file with all necessary settings for runing in IIS. 
How to configure IIS application to host ASP.NET Core site please learn more in the official Microsoft ASP.NET Core documentation 
[Host ASP.NET Core on Windows with IIS](https://docs.microsoft.com/en-us/aspnet/core/publishing/iis)


# License
Copyright (c) Virto Solutions LTD.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
