using System;
using System.IO;
using LibSassHost;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.LiquidThemeEngine
{
    public class SassFileManager: ISassFileManager
    {
        private readonly IContentBlobProvider _contentBlobProvider;

        public bool SupportsConversionToAbsolutePath { get; } = false;

        public string CurrentPath { get; set; }

        public SassFileManager(IContentBlobProvider contentBlobProvider)
        {
            _contentBlobProvider = contentBlobProvider;
        }

        public string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(CurrentPath);
        }

        public bool FileExists(string path)
        {
            return _contentBlobProvider.PathExists(path);
        }

        public bool IsAbsolutePath(string path)
        {
            return false;
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
