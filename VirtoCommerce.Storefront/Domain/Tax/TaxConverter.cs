using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Tax;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{

    public static partial class TaxConverter
    {
        public static TaxRate ToTaxRate(this coreDto.TaxRate taxRateDto, Currency currency)
        {
            var result = new TaxRate(currency)
            {
                Rate = new Money(taxRateDto.Rate.Value, currency),
                PercentRate = (decimal)(taxRateDto.PercentRate ?? 0)
            };

            if (taxRateDto.Line != null)
            {
                result.Line = new TaxLine(currency)
                {
                    Code = taxRateDto.Line.Code,
                    Id = taxRateDto.Line.Id,
                    Name = taxRateDto.Line.Name,
                    Quantity = taxRateDto.Line.Quantity ?? 1,
                    TaxType = taxRateDto.Line.TaxType,
                    TypeName = taxRateDto.Line.TypeName,

                    Amount = new Money(taxRateDto.Line.Amount.Value, currency),
                    Price = new Money(taxRateDto.Line.Price.Value, currency)
                };
                if (taxRateDto.TaxDetails != null)
                {
                    result.Line.TaxDetails = taxRateDto.TaxDetails.Select(x => x.ToTaxDetail(currency)).ToList();
                }
            }

            return result;
        }

        public static coreDto.TaxEvaluationContext ToTaxEvaluationContextDto(this TaxEvaluationContext taxContext)
        {
            var retVal = new coreDto.TaxEvaluationContext();
            retVal.Code = taxContext.Code;
            retVal.Id = taxContext.Id;
            retVal.Type = taxContext.Type;

            if (taxContext.Address != null)
            {
                retVal.Address = taxContext.Address.ToCoreAddressDto();
            }

            retVal.Customer = taxContext?.Customer?.Contact?.ToCoreContactDto();
            if (retVal.Customer != null)
            {
                retVal.Customer.MemberType = "Contact";
            }

            if (taxContext.Currency != null)
            {
                retVal.Currency = taxContext.Currency.Code;
            }

            retVal.Lines = new List<coreDto.TaxLine>();
            if (!taxContext.Lines.IsNullOrEmpty())
            {
                retVal.Lines = taxContext.Lines.Select(x => x.ToTaxLineDto()).ToList();

            }
            return retVal;
        }
        public static coreDto.TaxLine ToTaxLineDto(this TaxLine taxLine)
        {
            return new coreDto.TaxLine
            {
                Id = taxLine.Id,
                Code = taxLine.Code,
                Name = taxLine.Name,
                Quantity = taxLine.Quantity,
                TaxType = taxLine.TaxType,
                Amount = (double)taxLine.Amount.Amount,
                Price = (double)taxLine.Price.Amount,
                TypeName = taxLine.TypeName
            };
        }

        public static TaxEvaluationContext ToTaxEvaluationContext(this WorkContext workContext, IEnumerable<Product> products = null)
        {
            var result = new TaxEvaluationContext(workContext.CurrentStore.Id);
            result.Id = workContext.CurrentStore.Id;
            result.Currency = workContext.CurrentCurrency;
            result.Type = "";

            result.Customer = workContext.CurrentUser;
            result.StoreTaxCalculationEnabled = workContext.CurrentStore.TaxCalculationEnabled;
            result.FixedTaxRate = workContext.CurrentStore.FixedTaxRate;

            result.Address = workContext.CurrentUser?.Contact?.DefaultBillingAddress;

            if (products != null)
            {
                result.Lines = products.SelectMany(x => x.ToTaxLines()).ToList();
            }
            return result;
        }

        public static TaxDetail ToTaxDetail(this coreDto.TaxDetail taxDetailDto, Currency currency)
        {
            return new TaxDetail(currency)
            {
                Name = taxDetailDto.Name,
                Amount = new Money(taxDetailDto.Amount ?? 0, currency),
                Rate = new Money(taxDetailDto.Rate ?? 0, currency),
            };
        }
    }
}
