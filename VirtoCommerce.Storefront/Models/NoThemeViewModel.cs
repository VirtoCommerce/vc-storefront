using System;
using System.Collections;
using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Models
{
    public class NoThemeViewModel
    {
        public NoThemeViewModel()
        {
            SearchedLocations = new List<string>();
        }
        public IEnumerable<string> SearchedLocations { get; set; }
    }
}
