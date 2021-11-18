using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model
{
    public interface ICountriesService
    {
        Task<IList<Country>> GetCountriesAsync();
    }
}
