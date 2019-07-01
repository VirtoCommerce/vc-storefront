using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using contentDto = VirtoCommerce.Storefront.AutoRestClients.ContentModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{


    public static partial class LinkListConverter
    {
        public static MenuLinkList ToMenuLinkList(this contentDto.MenuLinkList menuLinkListDto)
        {
            var result = new MenuLinkList
            {
                Id = menuLinkListDto.Id,
                Name = menuLinkListDto.Name?.Handelize(),
                StoreId = menuLinkListDto.StoreId,


                Language = string.IsNullOrEmpty(menuLinkListDto.Language) ? Language.InvariantLanguage : new Language(menuLinkListDto.Language)
            };

            if (menuLinkListDto.MenuLinks != null)
            {
                result.MenuLinks = menuLinkListDto.MenuLinks.Select(ToMenuLink).ToList();
            }

            return result;
        }

        public static MenuLink ToMenuLink(this contentDto.MenuLink menuLinkDto)
        {
            var result = new MenuLink();
            if (menuLinkDto.AssociatedObjectType != null)
            {
                if ("product" == menuLinkDto.AssociatedObjectType.ToLowerInvariant())
                {
                    result = new ProductMenuLink();
                }
                else if ("category" == menuLinkDto.AssociatedObjectType.ToLowerInvariant())
                {
                    result = new CategoryMenuLink();
                }
            }
            result.Id = menuLinkDto.Id;
            result.AssociatedObjectId = menuLinkDto.AssociatedObjectId;
            result.AssociatedObjectType = menuLinkDto.AssociatedObjectType;
            result.Priority = menuLinkDto.Priority ?? 0;
            result.Title = menuLinkDto.Title;
            result.Url = menuLinkDto.Url;
            return result;
        }
    }
}
