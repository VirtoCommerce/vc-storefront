using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;
using toolsDto = VirtoCommerce.Tools.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class ToolsConverter
    {
        public static toolsDto.UrlBuilderContext ToToolsContext(this WorkContext workContext)
        {
            return new toolsDto.UrlBuilderContext
            {
                CurrentUrl = workContext.RequestUrl?.ToString(),
                CurrentLanguage = workContext.CurrentLanguage?.CultureName,
                CurrentStore = workContext.CurrentStore.ToToolsStore(),
                AllStores = workContext.AllStores?.Select(ToToolsStore).ToList(),
            };
        }

        public static toolsDto.Store ToToolsStore(this Store store)
        {
            toolsDto.Store result = null;

            if (store != null)
            {
                result = new toolsDto.Store
                {
                    Id = store.Id,
                    Url = store.Url,
                    SecureUrl = store.SecureUrl,
                    Catalog = store.Catalog,
                    DefaultLanguage = store.DefaultLanguage.CultureName,
                    SeoLinksType = store.SeoLinksType.ToToolsSeoLinksType(),
                    Languages = store.Languages.Select(l => l.CultureName).ToList(),
                };
            }

            return result;
        }

        public static toolsDto.SeoLinksType ToToolsSeoLinksType(this SeoLinksType seoLinksType)
        {
            return EnumUtility.SafeParse(seoLinksType.ToString(), toolsDto.SeoLinksType.None);
        }
    }
}
