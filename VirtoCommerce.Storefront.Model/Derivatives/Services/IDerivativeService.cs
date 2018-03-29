using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Derivatives.Services
{
    public interface IDerivativeService
    {
        Task EvaluateProductDerivativeInfoAsync(List<Product> products, WorkContext workContext);
    }
}
