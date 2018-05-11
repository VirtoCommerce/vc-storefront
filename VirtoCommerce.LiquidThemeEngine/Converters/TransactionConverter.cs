using System;
using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model.Order;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class TransactionConverter
    {
        public static Transaction ToShopifyModel(this StorefrontModel.PaymentIn payment)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidTransaction(payment);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Transaction ToLiquidTransaction(StorefrontModel.PaymentIn payment)
        {
            var result = new Transaction();
            result.Amount = payment.Sum.Amount * 100;
            result.CreatedAt = payment.CreatedDate ?? default(DateTime);
            result.Gateway = payment.GatewayCode;
            result.Id = payment.Id;
            result.Kind = payment.OperationType;
            result.Name = payment.Number;
            result.Receipt = payment.OuterId;
            result.Status = payment.Status;
            result.StatusLabel = payment.Status;
            result.PaymentDetails = payment.Purpose;
            return result;
        }
    }
}