using System;

namespace VirtoCommerce.Storefront.Domain.ContentBlobProviders.SymbolicLinks
{
    public static partial class JunctionPoint
    {
        [Flags]
        private enum FileShare : uint
        {
            None = 0x00000000,
            Read = 0x00000001,
            Write = 0x00000002,
            Delete = 0x00000004,
        }
    }
}
