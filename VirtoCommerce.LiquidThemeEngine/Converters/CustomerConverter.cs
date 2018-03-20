using PagedList.Core;
using System.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Security;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class CustomerConverter
    {
        public static Customer ToShopifyModel(this User user, StorefrontModel.WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidCustomer(user, workContext, urlBuilder);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Customer ToLiquidCustomer(User user, StorefrontModel.WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new Customer
            {
                DefaultLanguage = user.DefaultLanguage,
                Email = user.Email,
                OperatorUserId = user.OperatorUserId,
                OperatorUserName = user.OperatorUserName,
                UserName = user.UserName
            };

            if (user.Orders != null)
            {
                result.Orders = new MutablePagedList<Order>((pageNumber, pageSize, sortInfos) =>
                {
                    user.Orders.Slice(pageNumber, pageSize, sortInfos);
                    return new StaticPagedList<Order>(user.Orders.Select(x => ToLiquidOrder(x, workContext.CurrentLanguage, urlBuilder)), user.Orders);
                }, user.Orders.PageNumber, user.Orders.PageSize);
            }

            if (user.QuoteRequests != null)
            {
                result.QuoteRequests = new MutablePagedList<QuoteRequest>((pageNumber, pageSize, sortInfos) =>
                {
                    user.QuoteRequests.Slice(pageNumber, pageSize, sortInfos);
                    return new StaticPagedList<QuoteRequest>(user.QuoteRequests.Select(x => ToLiquidQuoteRequest(x)), user.QuoteRequests);
                }, user.QuoteRequests.PageNumber, user.QuoteRequests.PageSize);
            }

            var contact = user?.Contact?.Value;
            if (contact != null)
            {
                result.AcceptsMarketing = contact.AcceptsMarketing;
                result.FirstName = contact.FirstName;
                result.LastName = contact.LastName;
                result.MiddleName = contact.MiddleName;
                result.Name = contact.FullName;
                result.TimeZone = contact.TimeZone;
                if (contact.DefaultAddress != null)
                {
                    result.DefaultAddress = ToLiquidAddress(contact.DefaultAddress);
                }
                if (contact.DefaultBillingAddress != null)
                {
                    result.DefaultBillingAddress = ToLiquidAddress(contact.DefaultBillingAddress);
                }
                if (contact.DefaultShippingAddress != null)
                {
                    result.DefaultShippingAddress = ToLiquidAddress(contact.DefaultShippingAddress);
                }

                if (contact.Addresses != null)
                {
                    var addresses = contact.Addresses.Select(a => ToLiquidAddress(a)).ToList();
                    result.Addresses = new MutablePagedList<Address>(addresses);
                }

                if (contact.DynamicProperties != null)
                {
                    result.Metafields = new MetaFieldNamespacesCollection(new[] { new MetafieldsCollection("dynamic_properties", workContext.CurrentLanguage, contact.DynamicProperties) });
                }
            }
            

            return result;
        }
    }
}
