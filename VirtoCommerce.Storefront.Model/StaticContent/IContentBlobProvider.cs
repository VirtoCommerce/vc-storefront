using Microsoft.Extensions.Primitives;
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
        /// <summary>
        /// Check a given path exist.
        /// </summary>
        /// <param name="path">Relative or absolute path that identifies the file or folder.</param>
        /// <returns>The flag is path exist.</returns>
        bool PathExists(string path);
        /// <summary>
        /// Opens the given path for reading
        /// </summary>
        /// <param name="path">Relative or absolute path that identifies the file or folder.</param>
        /// <returns>stream</returns>
        Stream OpenRead(string path);
        /// <summary>
        /// Opens the given path for writing
        /// </summary>
        /// <param name="path">Relative or absolute path that identifies the file or folder.</param>
        /// <returns>stream</returns>
        Stream OpenWrite(string path);

        /// <summary>
        /// Search in given by <paramref name="path"/> location all occurrences with <paramref name="searchPattern"/>
        /// </summary>
        /// <param name="path">Relative or absolute path that identifies the file or folder.</param>
        /// <param name="searchPattern">Match pattern</param>
        /// <param name="recursive">Flag for recursive search</param>
        /// <returns>Paths for found files or folders</returns>
        IEnumerable<string> Search(string path, string searchPattern, bool recursive);

        /// <summary>
        /// Creates a <see cref="IChangeToken"/> for the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Path string used to determine what files or folders to monitor. Example: **/*.cs, *.*, subFolder/**/*.cshtml.</param>
        /// <returns>An <see cref="IChangeToken"/> that is notified when a file matching <paramref name="path"/> is added, modified or deleted.</returns>
        IChangeToken Watch(string path);
    }
}
