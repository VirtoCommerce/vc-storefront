using System;
using System.Linq;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Contracts;
using derivativesDto = VirtoCommerce.Storefront.AutoRestClients.DerivativeContractsModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain.Derivatives
{
    public static class DerivativeConverterExtension
    {
        public static DerivativeConverter DerivativeConverterInstance => new DerivativeConverter();

        public static DerivativeContract ToDerivativeContract(this derivativesDto.DerivativeContract dto)
        {
            return DerivativeConverterInstance.ToDerivativeContract(dto);
        }

        public static DerivativeContractItem ToDerivativeContractItem(this derivativesDto.DerivativeContractItem dto)
        {
            return DerivativeConverterInstance.ToDerivativeContractItem(dto);
        }

        public static DerivativeContractInfo ToDerivativeInfo(this derivativesDto.DerivativeContractInfo dto)
        {
            return DerivativeConverterInstance.ToDerivativeInfo(dto);
        }

        public static derivativesDto.DerivativeContractSearchCriteria ToSearchCriteriaDto(this DerivativeContractSearchCriteria criteria, WorkContext workContext)
        {
            return DerivativeConverterInstance.ToSearchCriteriaDto(criteria, workContext);
        }

        public static derivativesDto.DerivativeContractItemSearchCriteria ToSearchCriteriaDto(this DerivativeContractItemSearchCriteria criteria, WorkContext workContext)
        {
            return DerivativeConverterInstance.ToSearchCriteriaDto(criteria, workContext);
        }

        public static derivativesDto.DateTimeRange ToDateTimeRange(this DateTimeRange range)
        {
            return DerivativeConverterInstance.ToDateTimeRange(range);
        }
    }

    public partial class DerivativeConverter
    {
        public virtual DerivativeContract ToDerivativeContract(derivativesDto.DerivativeContract dto)
        {
            var result = new DerivativeContract
            {
                Id = dto.Id,
                MemberId = dto.MemberId,
                Type = EnumUtility.SafeParse(dto.Type, DerivativeContractType.Forward),
                StartDate = dto.StartDate ?? default(DateTime),
                EndDate = dto.EndDate,
                IsActive = dto.IsActive == true
            };

            return result;
        }

        public virtual DerivativeContractItem ToDerivativeContractItem(derivativesDto.DerivativeContractItem dto)
        {
            var result = new DerivativeContractItem
            {
                DerivativeContractId = dto.DerivativeContractId,
                FulfillmentCenterId = dto.FulfillmentCenterId,
                ProductId = dto.ProductId,
                ContractSize = (decimal)dto.ContractSize,
                PurchasedQuantity = (decimal)dto.PurchasedQuantity,
                RemainingQuantity = (decimal)dto.RemainingQuantity
            };

            return result;
        }

        public virtual DerivativeContractInfo ToDerivativeInfo(derivativesDto.DerivativeContractInfo dto)
        {
            var result = new DerivativeContractInfo
            {
                ProductId = dto.ProductId,
                Type = EnumUtility.SafeParse(dto.Type, DerivativeContractType.Forward),
                ContractSize = (decimal)dto.ContractSize,
                PurchasedQuantity = (decimal)dto.PurchasedQuantity,
                RemainingQuantity = (decimal)dto.RemainingQuantity
            };
            return result;
        }

        public virtual derivativesDto.DerivativeContractSearchCriteria ToSearchCriteriaDto(DerivativeContractSearchCriteria criteria, WorkContext workContext)
        {
            var result = new derivativesDto.DerivativeContractSearchCriteria
            {
                Types = criteria.Types?.Select(x => x.ToString()).ToList(),
                MemberIds = new[] { workContext.CurrentUser.ContactId },
                StartDateRange = criteria.StartDateRange?.ToDateTimeRange(),
                EndDateRange = criteria.EndDateRange?.ToDateTimeRange(),
                OnlyActive = criteria.OnlyActive,
                Sort = criteria.SortBy,
                Skip = criteria.Start,
                Take = criteria.PageSize
            };

            return result;
        }

        public virtual derivativesDto.DerivativeContractItemSearchCriteria ToSearchCriteriaDto(DerivativeContractItemSearchCriteria criteria, WorkContext workContext)
        {
            var result = new derivativesDto.DerivativeContractItemSearchCriteria
            {
                DerivativeContractIds = criteria.DerivativeContractIds,
                Types = criteria.Types?.Select(x => x.ToString()).ToList(),
                MemberIds = new[] { workContext.CurrentUser.ContactId },
                FulfillmentCenterIds = criteria.FulfillmentCenterIds,
                ProductIds = criteria.ProductIds,
                StartDateRange = criteria.StartDateRange?.ToDateTimeRange(),
                EndDateRange = criteria.EndDateRange?.ToDateTimeRange(),
                OnlyActive = criteria.OnlyActive,
                Sort = criteria.SortBy,
                Skip = criteria.Start,
                Take = criteria.PageSize
            };

            return result;
        }

        public virtual derivativesDto.DateTimeRange ToDateTimeRange(DateTimeRange range)
        {
            var result = new derivativesDto.DateTimeRange
            {
                FromDate = range.FromDate,
                ToDate = range.ToDate,
                IncludeFrom = range.IncludeFrom,
                IncludeTo = range.IncludeTo
            };
            return result;
        }
    }
}
