using System;
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
    }
}
