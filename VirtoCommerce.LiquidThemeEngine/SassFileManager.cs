using System;
using System.IO;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.LiquidThemeEngine
{
    public class SassFileManager : ISassFileManager
    {
        private readonly IContentBlobProvider _contentBlobProvider;

        public bool SupportsConversionToAbsolutePath { get; } = false;

        public string CurrentDirectory { get; set; }

        public SassFileManager(IContentBlobProvider contentBlobProvider)
        {
            _contentBlobProvider = contentBlobProvider;
        }

        public string GetCurrentDirectory() => CurrentDirectory;

        public bool FileExists(string path)
        {
            // Workaround for directories
            if (string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                return false;
            }
            return _contentBlobProvider.PathExists(path);
        }

        public bool IsAbsolutePath(string path)
        {
            return Path.GetDirectoryName(path).StartsWith(CurrentDirectory);
        }

        public string ToAbsolutePath(string path)
        {
            throw new NotImplementedException();
        }

        public string ReadFile(string path)
        {
            return _contentBlobProvider.OpenRead(path).ReadToString();
        }
    }
}
