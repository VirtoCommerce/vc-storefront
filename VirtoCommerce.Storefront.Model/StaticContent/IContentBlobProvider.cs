using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    /// <summary>
    /// This abstraction represent facade to access theme blob resources
    /// </summary>
    public interface IContentBlobProvider
    {
        event FileSystemEventHandler Changed;
        event RenamedEventHandler Renamed;
        bool PathExists(string path);
        Stream OpenRead(string path);
        Stream OpenWrite(string path);
        IEnumerable<string> Search(string path, string searchPattern, bool recursive);
    }

    public interface IStaticContentBlobProvider : IContentBlobProvider
    {
    }
    public interface IThemesContentBlobProvider : IContentBlobProvider
    {
    }
}
