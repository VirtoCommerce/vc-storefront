using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public interface IHasSettings
    {
        IMutablePagedList<SettingEntry> Settings { get; }
    }
}
