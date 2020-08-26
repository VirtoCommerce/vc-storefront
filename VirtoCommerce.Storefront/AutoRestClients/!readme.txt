See there, how to generate AutoRest-generated clients with the Platform v.3:
https://github.com/VirtoCommerce/vc-platform/blob/dev/docs/developer-guide/using-autorest-with-v3.md

Simplest example to refresh all clients:

$modules = @('Platform', 'Cart', 'Catalog', 'Content', 'Core', 'Customer', 'Inventory', 'Marketing', 'Notifications', 'Orders', 'Payment', 'Pricing', 'Shipping', 'Sitemaps', 'Store', 'Subscription', 'Tax')
$modules.ForEach( { autoRest VirtoCommerce.Storefront\AutoRestClients\array-in-query-fix.yml --version=3.0.6274 --v3 --debug --input-file=http://localhost:10645/docs/VirtoCommerce.$_/swagger.json --output-folder=.\VirtoCommerce.Storefront\AutoRestClients --output-file=$_`ModuleApi.cs --namespace=VirtoCommerce.Storefront.AutoRestClients.$_`ModuleApi --override-client-name=$_`ModuleClient --add-credentials --csharp })

