using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.LinkList.Services
{
    public interface IMenuLinkListService
    {
        IList<MenuLinkList> LoadAllStoreLinkLists(Store store, Language language);
        Task<IList<MenuLinkList>> LoadAllStoreLinkListsAsync(Store store, Language language);
    }
}
