using LibSassHost;

namespace VirtoCommerce.LiquidThemeEngine
{
    public interface ISassFileManager: IFileManager
    {
        string CurrentDirectory { get; set; }
    }
}
