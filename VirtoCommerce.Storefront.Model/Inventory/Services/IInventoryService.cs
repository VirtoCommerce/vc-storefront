﻿using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Inventory.Services
{
    public interface IInventoryService
    {
        Task EvaluateProductInventoriesAsync(IEnumerable<Product> products, WorkContext workContext);
    }
}
