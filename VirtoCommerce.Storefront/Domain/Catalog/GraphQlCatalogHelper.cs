using System.Linq;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Domain.Catalog
{
    public static class GraphQlCatalogHelper
    {
        private const int _maxDepth = 2;

        public static string GetAllProductFields(string cultureName, string currencyCode, int currentDepth = 0)
        {
            var result = $@"
                id code
                category {{ {AllCategoryFields} parent {{{AllCategoryFields}}} }}
                catalogId
                name
                metaTitle
                metaDescription
                metaKeywords
                brandName
                slug
                imgSrc
                productType
                descriptions {{
                    content
                    id
                    languageCode
                    reviewType
                }}
                masterVariation {{ {GetAllVariationFields(cultureName, currencyCode)} }}
                properties {{ {PropertyFields} }}
                associations {{
                    items {{
                        associatedObjectId
                        {(++currentDepth < _maxDepth ? @$"product {{ {GetAllProductFields(cultureName, currencyCode, currentDepth)} }}" : string.Empty)}
                        quantity
                    }}
                    totalCount
                }}
                variations {{ {GetAllVariationFields(cultureName, currencyCode)} }}";

            return result;
        }

        public static string GetAllVariationFields(string cultureName, string currencyCode)
        {
            var result = $@"id
                    code
                    images {{ id url name }}
                    assets {{ id size url }}
                    prices( cultureName:""{cultureName}"" ) {{
                        list {{ {MoneyFields} }}
                        listWithTax {{ {MoneyFields} }}
                        sale {{ {MoneyFields} }}
                        saleWithTax {{ {MoneyFields} }}
                        minQuantity
                        pricelistId
                        tierPrices {{
                            price {{ {MoneyFields} }}
                            priceWithTax {{ {MoneyFields} }}
                            quantity
                        }}
                        validFrom
                        validUntil
                        currency
                        discounts {{
                            coupon
                            description
                            promotion {{
                                id
                                description
                                name
                                type
                            }}
                            promotionId
                            amount(cultureName:""{cultureName}"", currencyCode: ""{currencyCode}"") {{ {MoneyFields} }}
                        }}
                    }}
                    availabilityData {{
                        availableQuantity
                        inventories {{ inStockQuantity fulfillmentCenterId fulfillmentCenterName allowPreorder allowBackorder }}
                    }}
                    properties {{ {PropertyFields} }}";

            return result;
        }

        public const string MoneyFields = "amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency";
        public const string PropertyFields = "id name valueType value valueId";
        public const string AllCategoryFields = "id code name hasParent slug";

        public static string GetProducts(this ICatalogService catalogService, string[] ids, string cultureName, string currencyCode, string selectedFields = null)
        => $@"
        {{
            products(productIds: [{string.Join(',', ids.Select(x => $"\"{x}\""))}])
            {{
                items
                {{
                    { selectedFields ?? GetAllProductFields(cultureName, currencyCode) }
                }}
            }}
        }}
        ";

        public static string GetCategoriesQuery(this ICatalogService catalogService, string[] ids, string selectedFields = null)
        => $@"
        {{
            categories(categoryIds: [{string.Join(',', ids.Select(x => $"\"{x}\""))}])
            {{
                items
                {{
                    { selectedFields ?? AllCategoryFields } parent {{ {AllCategoryFields} }}
                }}
            }}
        }}
        ";
    }
}
