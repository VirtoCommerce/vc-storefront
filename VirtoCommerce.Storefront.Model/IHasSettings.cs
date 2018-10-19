using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model
{
    public interface IHasSettings
    {
        IList<SettingEntry> Settings { get; }
    }
}
