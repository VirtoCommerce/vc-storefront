# VirtoCommerce Storefront Core
VirtoCommerce Storefront represents the official online shopping website based on VirtoCommerce Platform written on ASP.NET Core. The website is a client application for VC Platform and uses only public APIs while communicating.

Technologies and frameworks used:
* ASP.NET MVC Core 2.0.0 on .NET Core 2.0.0
* ASP.NET Identity Core 2.0.0
* Microsoft AutoRest
* Liquid view engine

Key features:
* multiple themes (full theme customization in Liquid templates)
* dynamic content
* B2C, B2B scenarios (quotes)
* configurable shipping and payment options
* SEO
* multiple languages and currencies

![Storefront UI](https://cloud.githubusercontent.com/assets/5801549/15822429/682f32d8-2bfe-11e6-9ddf-562b400afeb1.png)

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
https://docs.microsoft.com/en-us/dotnet/core/windows-prerequisites

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
      "Url": "http://localhost/admin" <!-- Virto Commerce platform manager url -->,
	  <!-- HMAC authentification user credentials on whose behalf the API calls will be made.
      "AppId": "27e0d789f12641049bd0e939185b4fd2" 
      "SecretKey": "34f0a3c12c9dbb59b63b5fece955b7b2b9a3b20f84370cba1524dd5c53503a2e2cb733536ecf7ea1e77319a47084a3a2c9d94d36069a432ecc73b72aeba6ea78",
    }
	...
```
ASP.NET Core represents a new tools a **Secret Manager tool**, which allows in development to keep secrets out of your code. 
You can find more about them [here](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio)

### Configure themes 
Storefront  **appsettings.json** file contains **ContentConnectionString** setting with pointed to folder with actual themes and pages content
```
...
"ConnectionStrings": {
    <!-- For themes stored in local file system  -->
    "ContentConnectionString": "provider=LocalStorage;rootPath=~/cms-content"
	<!-- For themes stored in azure blob storage  -->
    <!--<add name="ContentConnectionString" connectionString="provider=AzureBlobStorage;rootPath=cms-content;DefaultEndpointsProtocol=https;AccountName=yourAccountName;AccountKey=yourAccountKey" />-->
  },
...
```
You can set this connection string in one of the following ways:
1. If you have already have installed  platform with sample data, your platform already contains `~/App_Data/cms-content` folder with themes for smaple stores
and you need only to make symbolic link to this folder by this command:
```
mklink /d C:\vc-storefront\VirtoCommerce.Storefront\wwwroot\cms-content C:\vc-platform\VirtoCommerce.Platform.Web\App_Data\cms-content
```
2. If you did not install sample data with your platform, you need to create new store in platform manager and download themes as it described in this article 
[Theme development](https://virtocommerce.com/docs/vc2devguide/working-with-storefront/theme-development)

### Host on Windows with IIS 
VirtoCommerce.Storefront project already include the **web.config** file with all necessary settings for runing in IIS. 
How to configure IIS application to host ASP.NET Core site please learn more in the official Microsoft ASP.NET Core documentation 
https://docs.microsoft.com/en-us/aspnet/core/publishing/iis

# License
Copyright (c) Virtosoftware Ltd.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
