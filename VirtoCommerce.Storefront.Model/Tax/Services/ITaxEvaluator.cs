using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Tax.Services
{
    public interface ITaxEvaluator
    {
        Task EvaluateTaxesAsync(TaxEvaluationContext context, IEnumerable<ITaxable> owners);
        void EvaluateTaxes(TaxEvaluationContext context, IEnumerable<ITaxable> owners);
    }
}
