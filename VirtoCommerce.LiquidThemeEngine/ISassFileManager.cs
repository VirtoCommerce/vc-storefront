using LibSassHost;

namespace VirtoCommerce.LiquidThemeEngine
{
    public interface ISassFileManager: IFileManager
    {
        string CurrentPath { get; set; }
    }
}
