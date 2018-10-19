using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public interface ICurrencyService
    {
        Task<Currency[]> GetAllCurrenciesAsync(Language language);
    }
}
