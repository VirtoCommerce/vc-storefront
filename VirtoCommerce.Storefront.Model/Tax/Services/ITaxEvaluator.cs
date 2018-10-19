using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Tax.Services
{
    public interface ITaxEvaluator
    {
        Task EvaluateTaxesAsync(TaxEvaluationContext context, IEnumerable<ITaxable> owners);
    }
}
