
Install Node.js v4.0.0 or above (we recommend latest LTS version https://nodejs.org/en/)
Install latest AutoRest globally via 'npm install -g autorest' command (2.0.4283 was used last time)
 
1. Open Tools > NuGet Package Manager > Package Manager Console
2. Run the following commands to generate API clients:

$modules = @('Platform', 'Cart', 'Catalog', 'Content', 'Core', 'Customer', 'Inventory', 'Marketing', 'Notifications', 'Orders', 'Payment', 'Pricing', 'Shipping', 'Sitemaps', 'Store', 'Subscription', 'Tax')
$modules.ForEach( { autoRest --debug --input-file=http://localhost:10645/docs/VirtoCommerce.$_/swagger.json --output-folder=VirtoCommerce.Storefront\AutoRestClients --output-file=$_`ModuleApi.cs --namespace=VirtoCommerce.Storefront.AutoRestClients.$_`ModuleApi --override-client-name=$_`ModuleClient --add-credentials --csharp })


Troubleshooting

See AutoRest guide here:
https://github.com/Azure/autorest/blob/master/docs/developer/guide/building-code.md#strong-name-validation-errors
