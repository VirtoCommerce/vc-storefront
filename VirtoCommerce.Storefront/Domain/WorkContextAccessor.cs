using System.Threading;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain
{
    public class WorkContextAccessor : IWorkContextAccessor
    {
        private static AsyncLocal<WorkContext> _workContextCurrent = new AsyncLocal<WorkContext>();

        public WorkContext WorkContext
        {
            get
            {
                return _workContextCurrent.Value;
            }
            set
            {
                _workContextCurrent.Value = value;
            }
        }

    }
}
