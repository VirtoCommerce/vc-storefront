using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public interface ICurrencyService
    {
        Task<Currency[]> GetAllCurrenciesAsync(Language language);
    }
}
