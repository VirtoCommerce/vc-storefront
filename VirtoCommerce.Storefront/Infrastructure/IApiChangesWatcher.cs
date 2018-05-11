using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public interface IApiChangesWatcher
    {
        IChangeToken CreateChangeToken();
    }
}
