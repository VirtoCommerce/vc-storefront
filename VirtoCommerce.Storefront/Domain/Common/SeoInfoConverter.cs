using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Contracts.Catalog;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class SeoInfoConverter
    {
        public static SeoInfo ToSeoInfo(this coreDto.SeoInfo seoDto)
        {
            var retVal = new SeoInfo
            {
                MetaDescription = seoDto.MetaDescription,
                MetaKeywords = seoDto.MetaKeywords,

                Slug = seoDto.SemanticUrl,
                Title = seoDto.PageTitle,
                Language = string.IsNullOrEmpty(seoDto.LanguageCode) ? Language.InvariantLanguage : new Language(seoDto.LanguageCode)
            };
            return retVal;
        }

        public static SeoInfo ToSeoInfo(this SeoInfoDto seoDto)
        {
            var retVal = new SeoInfo
            {
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
