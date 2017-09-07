using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model
{
    public interface ICountriesService
    {
        IEnumerable<Country> GetCountries();
    }
}
