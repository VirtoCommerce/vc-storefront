using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Services
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
