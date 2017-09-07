using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Stores
{
    public interface IStoreService
    {
        Task<Store[]> GetAllStoresAsync();
    }
}
