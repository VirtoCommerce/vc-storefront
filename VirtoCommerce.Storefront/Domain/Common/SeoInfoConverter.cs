using VirtoCommerce.Storefront.Model;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class SeoInfoConverter
    {
        public static SeoInfo ToSeoInfo(this coreDto.SeoInfo seoDto)
        {
            var retVal = new SeoInfo
            {
                Id = seoDto.Id,
                ObjectId = seoDto.ObjectId,
                ObjectType = seoDto.ObjectType,
                IsActive = seoDto.IsActive,

                MetaDescription = seoDto.MetaDescription,
                MetaKeywords = seoDto.MetaKeywords,

                Slug = seoDto.SemanticUrl,
                Title = seoDto.PageTitle,
                Language = string.IsNullOrEmpty(seoDto.LanguageCode) ? Language.InvariantLanguage : new Language(seoDto.LanguageCode)
            };
            return retVal;
        }

    }
}
