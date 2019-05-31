using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using paymentDto = VirtoCommerce.Storefront.AutoRestClients.PaymentModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static partial class PaymentConverter
    {
        public static PaymentMethod ToPaymentMethod(this paymentDto.PaymentMethod paymentMethodDto, Currency currency)
        {
            var retVal = new PaymentMethod(currency)
            {
                Code = paymentMethodDto.Code,
                Description = paymentMethodDto.Currency,
                IsAvailableForPartial = paymentMethodDto.IsAvailableForPartial ?? false,
                LogoUrl = paymentMethodDto.LogoUrl,
                Name = paymentMethodDto.TypeName,
                PaymentMethodGroupType = paymentMethodDto.PaymentMethodGroupType,
                PaymentMethodType = paymentMethodDto.PaymentMethodType,
                TaxType = paymentMethodDto.TaxType,

                Priority = paymentMethodDto.Priority ?? 0
            };

            if (paymentMethodDto.Settings != null)
            {
                retVal.Settings = paymentMethodDto.Settings.Where(x => !x.ValueType.EqualsInvariant("SecureString")).Select(x => x.JsonConvert<AutoRestClients.PlatformModuleApi.Models.Setting>().ToSettingEntry()).ToList();
            }

            retVal.Currency = currency;
            retVal.Price = new Money(paymentMethodDto.Price ?? 0, currency);
            retVal.DiscountAmount = new Money(paymentMethodDto.DiscountAmount ?? 0, currency);
            retVal.TaxPercentRate = (decimal?)paymentMethodDto.TaxPercentRate ?? 0m;

            if (paymentMethodDto.TaxDetails != null)
            {
                retVal.TaxDetails = paymentMethodDto.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }

            return retVal;
        }

        public static TaxDetail ToTaxDetail(this paymentDto.TaxDetail dto, Currency currency)
        {
            var result = new TaxDetail(currency)
            {
                Amount = new Money(dto.Amount ?? 0, currency),
                Rate = new Money(dto.Rate ?? 0, currency),
                Name = dto.Name
            };
            return result;
        }
    }
}
