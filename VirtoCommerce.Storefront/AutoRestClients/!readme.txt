How to generate AutoRest-generated clients with the Platform v.3
read the documentation in 'docs' folder of vc-platform v.3
https://github.com/VirtoCommerce/vc-platform/blob/release/3.0.0/docs/using-autorest-with-v3.md

----------------------------------------------------------------------------------------------

Install Node.js v4.0.0 or above (we recommend latest LTS version https://nodejs.org/en/)
Install latest AutoRest globally via 'npm install -g autorest' command (2.0.4283 was used last time)
 
1. Open Tools > NuGet Package Manager > Package Manager Console
2. Run the following commands to generate API clients:

$modules = @('Cache','Cart','Catalog','Content','Core','Customer','Inventory','Marketing','Orders','Platform','Pricing','Quote','Sitemaps','Store','Subscription')
$modules.ForEach( { autoRest -Input http://localhost/admin/docs/VirtoCommerce.$_/v1  -OutputFileName $_`ModuleApi.cs -Namespace VirtoCommerce.Storefront.AutoRestClients.$_`ModuleApi -ClientName $_`ModuleApiClient -OutputDirectory VirtoCommerce.Storefront\AutoRestClients -AddCredentials true -UseDateTimeOffset false })


Troubleshooting:
To fix error reading from url 'Could not read 'http://...'' it's recommended to create swagger file manually,
and then use it as parameter in -Input, e.g.:

1. iwr http://localhost/admin/docs/CustomerReviewsModule.Web/v1 -outfile swagger.CustomerReviewsModule.Web.json
2. $modules = @('CustomerReviewsModule.Web')
   $modules.ForEach( { autoRest -Input swagger.$_.json  -OutputFileName $_`ModuleApi.cs -Namespace VirtoCommerce.Storefront.AutoRestClients.$_`ModuleApi -ClientName $_`ModuleApiClient -OutputDirectory VirtoCommerce.Storefront\AutoRestClients -AddCredentials true -UseDateTimeOffset false })


See AutoRest guide here:
https://github.com/Azure/autorest/blob/master/docs/developer/guide/building-code.md#strong-name-validation-errors
