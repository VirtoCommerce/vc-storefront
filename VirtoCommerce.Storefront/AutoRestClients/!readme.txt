Install latest AutoRest v1 globally via 'npm install -g autorest@1.2.2' command
 
1. Open Tools > NuGet Package Manager > Package Manager Console
2. Run the following commands to generate API clients:

$modules = @('Cache','Cart','Catalog','Content','Core','Customer','Inventory','Marketing','Orders','Platform','Pricing','Quote','Sitemaps','Store','Subscription')
$modules.ForEach( { autoRest -Input http://localhost/admin/docs/VirtoCommerce.$_/v1  -OutputFileName $_`ModuleApi.cs -Namespace VirtoCommerce.Storefront.AutoRestClients.$_`ModuleApi -ClientName $_`ModuleApiClient -OutputDirectory VirtoCommerce.Storefront\AutoRestClients -AddCredentials true -UseDateTimeOffset false })


Troubleshooting

See AutoRest guide here:
https://github.com/Azure/autorest/blob/master/docs/developer/guide/building-code.md#strong-name-validation-errors



$modules = @('Store')
$modules.ForEach( { autoRest -Input http://localhost/admin/docs/VirtoCommerce.$_/v1  -OutputFileName $_`ModuleApi_2.cs -Namespace VirtoCommerce.Storefront.AutoRestClients.$_`ModuleApi_2 -ClientName $_`ModuleApi_2Client -OutputDirectory VirtoCommerce.Storefront\AutoRestClients -UseDateTimeOffset false })
