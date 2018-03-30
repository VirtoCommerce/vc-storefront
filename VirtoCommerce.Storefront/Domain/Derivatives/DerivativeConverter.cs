using System;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Derivatives;
using derivativesDto = VirtoCommerce.Storefront.AutoRestClients.DerivativesModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain.Derivatives
{
    public static class DerivativeConverter
    {
        public static Derivative ToDerivative(this derivativesDto.Derivative dto)
        {
            var result = new Derivative
            {
                EndDate = dto.EndDate,
                Id = dto.Id,
                IsActive = dto.IsActive == true,
                MemberId = dto.MemberId,
                StartDate = dto.StartDate ?? default(DateTime),
                Type = EnumUtility.SafeParse(dto.Type, DerivativeType.Forward)
            };

            return result;
        }
        public static DerivativeItem ToDerivativeItem(this derivativesDto.DerivativeItem dto)
        {
            var result = new DerivativeItem
            {
                DerivativeId = dto.DerivativeId,
                FulfillmentCenterId = dto.FulfillmentCenterId,
                ProductId = dto.ProductId,
                ContractSize = (decimal)dto.ContractSize,
                PurchasedQuantity = (decimal)dto.PurchasedQuantity,
                RemainingQuantity = (decimal)dto.RemainingQuantity
            };

            return result;
        }

        public static derivativesDto.DerivativeSearchCriteria ToSearchCriteriaDto(this DerivativeSearchCriteria criteria, WorkContext workContext)
        {
            var result = new derivativesDto.DerivativeSearchCriteria
            {
                FulfillmentCenterIds = workContext.CurrentStore.FulfilmentCenters.Select(x => x.Id).ToArray(),
                LanguageCode = workContext.CurrentLanguage.TwoLetterLanguageName,
                MemberIds = new[] { workContext.CurrentUser.ContactId },
                OnlyActive = criteria.OnlyActive,
                Types = criteria.Types?.Select(x => x.ToString()).ToList(),

                Skip = criteria.Start,
                Take = criteria.PageSize,
                Sort = criteria.SortBy
            };

            return result;
        }

        public static derivativesDto.DerivativeSearchCriteria ToSearchCriteriaDto(this DerivativeItemSearchCriteria criteria, WorkContext workContext)
        {
            var result = new derivativesDto.DerivativeSearchCriteria
            {
                FulfillmentCenterIds = workContext.CurrentStore.FulfilmentCenters.Select(x => x.Id).ToArray(),
                LanguageCode = workContext.CurrentLanguage.TwoLetterLanguageName,
                MemberIds = new[] { workContext.CurrentUser.ContactId },
                OnlyActive = criteria.OnlyActive,
                ProductIds = criteria.ProductIds,
                Types = criteria.Types?.Select(x => x.ToString()).ToList(),

                Skip = criteria.Start,
                Take = criteria.PageSize,
                Sort = criteria.SortBy
            };

            return result;
        }
    }
}
