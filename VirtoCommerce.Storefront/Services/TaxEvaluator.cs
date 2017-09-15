using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Tax;
using VirtoCommerce.Storefront.Model.Tax.Services;
using coreService = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;

namespace VirtoCommerce.Storefront.Services
{
    public class TaxEvaluator : ITaxEvaluator
    {
        private readonly ICommerce _commerceApi;

        public TaxEvaluator(ICommerce commerceApi)
        {
            _commerceApi = commerceApi;
        }

        #region ITaxEvaluator Members
        public virtual async Task EvaluateTaxesAsync(TaxEvaluationContext context, IEnumerable<ITaxable> owners)
        {
            IList<coreService.TaxRate> taxRates = new List<coreService.TaxRate>();
            if (context.StoreTaxCalculationEnabled)
            {
                taxRates = await _commerceApi.EvaluateTaxesAsync(context.StoreId, context.ToTaxEvaluationContextDto());
            }
            InnerEvaluateTaxes(taxRates, owners);
        }

        public virtual void EvaluateTaxes(TaxEvaluationContext context, IEnumerable<ITaxable> owners)
        {
            IList<coreService.TaxRate> taxRates = new List<coreService.TaxRate>();
            if (context.StoreTaxCalculationEnabled)
            {
                taxRates = _commerceApi.EvaluateTaxes(context.StoreId, context.ToTaxEvaluationContextDto());
            }
            InnerEvaluateTaxes(taxRates, owners);
        }

        #endregion

        private static void InnerEvaluateTaxes(IList<coreService.TaxRate> taxRates, IEnumerable<ITaxable> owners)
        {
            if (taxRates == null)
            {
                return;
            }
            var taxRatesMap = owners.Select(x => x.Currency).Distinct().ToDictionary(x => x, x => taxRates.Select(r => r.ToTaxRate(x)).ToArray());

            foreach (var owner in owners)
            {
                owner.ApplyTaxRates(taxRatesMap[owner.Currency]);
            }
        }
    }
}
