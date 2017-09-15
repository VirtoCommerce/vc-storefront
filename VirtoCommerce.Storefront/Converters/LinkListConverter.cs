using System.Linq;
using VirtoCommerce.Storefront.Model;
using contentDto = VirtoCommerce.Storefront.AutoRestClients.ContentModuleApi.Models;

namespace VirtoCommerce.Storefront.Converters
{
    public static class LinkListConverterExtension
    {
        public static LinkListConverter LinkListConverterInstance
        {
            get
            {
                return new LinkListConverter();
            }
        }

        public static MenuLinkList ToMenuLinkList(this contentDto.MenuLinkList menuLinkListDto)
        {
            return LinkListConverterInstance.ToMenuLinkList(menuLinkListDto);
        }

        public static MenuLink ToMenuLink(this contentDto.MenuLink menuLinkDto)
        {
            return LinkListConverterInstance.ToMenuLink(menuLinkDto);
        }
    }

    public partial class LinkListConverter
    {
        public virtual MenuLinkList ToMenuLinkList(contentDto.MenuLinkList menuLinkListDto)
        {
            var result = new MenuLinkList();

            result.Id = menuLinkListDto.Id;
            result.Name = menuLinkListDto.Name;
            result.StoreId = menuLinkListDto.StoreId;
           

            result.Language = string.IsNullOrEmpty(menuLinkListDto.Language) ? Language.InvariantLanguage : new Language(menuLinkListDto.Language);

            if (menuLinkListDto.MenuLinks != null)
            {
                result.MenuLinks = menuLinkListDto.MenuLinks.Select(ToMenuLink).ToList();
            }

            return result;
        }

        public virtual MenuLink ToMenuLink(contentDto.MenuLink menuLinkDto)
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
